const express = require('express');
const fs = require('fs').promises;
const path = require('path');
const JSONbig = require('json-bigint')({ storeAsString: true });

const app = express();

// Используем json-bigint, чтобы не терять точность больших чисел
app.use(express.text({ type: 'application/json' }));
app.use((req, res, next) => {
  if (req.body && typeof req.body === 'string') {
    try {
      req.body = JSONbig.parse(req.body);
    } catch (e) {
      return res.status(400).json({ error: 'Invalid JSON' });
    }
  }
  next();
});

const UPLOAD_BASE = process.env.UPLOAD_PATH || '/app/uploads';

app.post('/api/files/merge', async (req, res) => {
    try {
        const { userId, fileId, parts, fileName } = req.body;
        
        // Защита от выхода за пределы каталога (path traversal)
        if (!fileName || typeof fileName !== 'string') {
            return res.status(400).json({ error: 'Invalid fileName parameter' });
        }

        // Блокируем попытки обхода пути
        if (fileName.includes('..') || fileName.includes('/') || fileName.includes('\\')) {
            console.error(`Заблокирована попытка обхода пути - fileName: ${fileName}, IP: ${req.ip}`);
            return res.status(400).json({ error: 'Invalid fileName: path traversal not allowed' });
        }

        // Очищаем имя файла: оставляем только буквы, цифры, дефис, подчёркивание и точки
        const sanitizedFileName = fileName.replace(/[^a-zA-Z0-9._-]/g, '_');
        if (sanitizedFileName !== fileName) {
            console.warn(`Имя файла очищено: "${fileName}" -> "${sanitizedFileName}"`);
        }
        
        console.log(`Merging file from local filesystem: userId=${userId}, fileId=${fileId}, parts=${parts}, fileName=${fileName}`);
        
        // Части файла хранятся в формате: {userId}-{actualFileId}-{partNumber}
        // fileId из command-server может отличаться от того, что использует file-server,
        // поэтому реальный fileId находим, перебирая файлы этого пользователя

        console.log(`Searching for files with userId: ${userId}`);
        
        // Получаем список всех файлов этого пользователя
        const allFiles = await fs.readdir(UPLOAD_BASE);
        const userFiles = allFiles.filter(f => f.startsWith(`${userId}-`));
        
        console.log(`Found ${userFiles.length} files for user ${userId}`);
        
        if (userFiles.length === 0) {
            return res.status(404).json({ error: 'No files found for user' });
        }
        
        // Группируем файлы по fileId (берём его из имени: userId-fileId-partNumber)
        const fileGroups = {};
        for (const file of userFiles) {
            const match = file.match(/^(\d+)-(-?\d+)-(\d+)$/);
            if (match) {
                const [, uid, fid, partNum] = match;
                if (!fileGroups[fid]) {
                    fileGroups[fid] = [];
                }

                fileGroups[fid].push({ file, partNum: parseInt(partNum) });
            }
        }
        
        console.log(`Found ${Object.keys(fileGroups).length} file groups:`, Object.keys(fileGroups));
        
        // Ищем группу с нужным количеством частей
        let actualFileId = null;
        let partFiles = null;

        for (const [fid, files] of Object.entries(fileGroups)) {
            if (files.length === parts) {
                actualFileId = fid;
                partFiles = files.sort((a, b) => a.partNum - b.partNum);
                console.log(`Found matching file group: fileId=${fid}, parts=${files.length}`);
                break;
            }
        }

        if (!actualFileId || !partFiles) {
            console.error(`No file group with ${parts} parts found`);
            console.log('Available groups:', Object.entries(fileGroups).map(([fid, files]) => `${fid} (${files.length} parts)`));
            return res.status(404).json({ error: `No file group with ${parts} parts found` });
        }
        
        // Проверяем, что все части на месте
        const partPaths = [];
        for (let i = 0; i < parts; i++) {
            const partFile = partFiles.find(f => f.partNum === i);
            if (!partFile) {
                console.error(`Missing part ${i}`);
                return res.status(404).json({ error: `Missing file part ${i}` });
            }
            partPaths.push(path.join(UPLOAD_BASE, partFile.file));
        }
        
        console.log(`All ${parts} parts found, merging...`);
        
        // Склеиваем части
        const mergedDir = path.join(UPLOAD_BASE, 'merged');
        await fs.mkdir(mergedDir, { recursive: true });

        // Используем очищенное имя файла
        const extension = sanitizedFileName.split('.').pop() || '';
        const mergedFileName = `${userId}-${actualFileId}${extension ? '.' + extension : ''}`;

        // Дополнительно убеждаемся, что итоговый путь остаётся внутри разрешённого каталога
        const mergedFilePath = path.join(mergedDir, mergedFileName);
        const resolvedPath = path.resolve(mergedFilePath);
        const resolvedMergedDir = path.resolve(mergedDir);

        if (!resolvedPath.startsWith(resolvedMergedDir)) {
            console.error(`Обнаружен выход за пределы каталога. Запрошенный путь: ${resolvedPath}`);
            return res.status(400).json({ error: 'Invalid file path' });
        }
        
        const writeStream = require('fs').createWriteStream(mergedFilePath);
        let totalSize = 0;
        
        // Ограничение на размер файла (максимум 2 ГБ)
        const MAX_FILE_SIZE = 2 * 1024 * 1024 * 1024; // 2 ГБ

        for (const partPath of partPaths) {
            const partData = await fs.readFile(partPath);

            // Проверяем размер перед записью
            if (totalSize + partData.length > MAX_FILE_SIZE) {
                console.error(`Превышен лимит размера файла - запрошенный размер: ${totalSize + partData.length}, лимит: ${MAX_FILE_SIZE}`);

                // Удаляем недописанный файл
                try {
                    writeStream.close();
                    await fs.unlink(mergedFilePath);
                } catch (e) {}
                
                return res.status(413).json({ error: 'File too large: Maximum size is 2GB' });
            }
            
            writeStream.write(partData);
            totalSize += partData.length;
        }
        
        console.log(`Проверка размера пройдена: ${totalSize} bytes (limit: ${MAX_FILE_SIZE})`);
        
        writeStream.end();
        
        await new Promise((resolve, reject) => {
            writeStream.on('finish', resolve);
            writeStream.on('error', reject);
        });
        
        console.log(`Merged ${parts} parts into ${mergedFileName}, total size: ${totalSize} bytes`);

        // Удаляем части после склейки
        for (const partPath of partPaths) {
            try {
                await fs.unlink(partPath);
            } catch (e) {
                console.error(`Failed to remove part ${partPath}:`, e.message);
            }
        }
        
        res.json({
            filePath: path.join('merged', mergedFileName),
            totalSize: totalSize
        });
        
    } catch (error) {
        console.error('Merge error:', error);
        res.status(500).json({ error: error.message });
    }
});

app.get('/health', (req, res) => {
    res.json({ status: 'ok' });
});

const PORT = process.env.PORT || 5000;
app.listen(PORT, () => {
    console.log(`File merge proxy listening on port ${PORT}`);
    console.log(`Upload base path: ${UPLOAD_BASE}`);
});
