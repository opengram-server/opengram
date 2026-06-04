import express from 'express';
import multer from 'multer';
import { promises as fs } from 'fs';
import path from 'path';
import zlib from 'zlib';
import { promisify } from 'util';
import { ObjectId } from 'mongodb';
import minioHelper from '../utils/minioHelper.js';
import { generateFileReference, generateAccessHash, generateDocumentId } from '../utils/telegramIds.js';

const gunzip = promisify(zlib.gunzip);
const router = express.Router();

const storage = multer.diskStorage({
    destination: async (req, file, cb) => {
        const uploadDir = 'uploads/attributes';
        await fs.mkdir(uploadDir, { recursive: true });
        cb(null, uploadDir);
    },
    filename: (req, file, cb) => {
        const uniqueSuffix = Date.now() + '-' + Math.round(Math.random() * 1E9);
        cb(null, `${file.fieldname}-${uniqueSuffix}${path.extname(file.originalname)}`);
    }
});

const upload = multer({
    storage,
    limits: { fileSize: 10 * 1024 * 1024 },
    fileFilter: (req, file, cb) => {
        const allowedExts = ['.png', '.tgs'];
        const ext = path.extname(file.originalname).toLowerCase();
        if (allowedExts.includes(ext)) {
            cb(null, true);
        } else {
            cb(new Error('Only .png and .tgs files are allowed'));
        }
    }
});

// Отдельная конфигурация multer для загрузки ZIP-архивов
const zipStorage = multer.diskStorage({
    destination: async (req, file, cb) => {
        const uploadDir = 'uploads/zip';
        await fs.mkdir(uploadDir, { recursive: true });
        cb(null, uploadDir);
    },
    filename: (req, file, cb) => {
        const uniqueSuffix = Date.now() + '-' + Math.round(Math.random() * 1E9);
        cb(null, `${file.fieldname}-${uniqueSuffix}.zip`);
    }
});

const uploadZip = multer({
    storage: zipStorage,
    limits: { fileSize: 50 * 1024 * 1024 }, // до 50 МБ для ZIP
    fileFilter: (req, file, cb) => {
        const ext = path.extname(file.originalname).toLowerCase();
        if (ext === '.zip') {
            cb(null, true);
        } else {
            cb(new Error('Only .zip files are allowed'));
        }
    }
});

// Приводит имя к единому виду для сопоставления: пробелы и дефисы -> подчёркивания,
// убирает известные префиксы и расширение
function normalizeName(name) {
    return String(name || '')
        .toLowerCase()
        .replace(/\\+/g, '/')
        .replace(/.*\//, '')                  // оставляем только имя файла
        .replace(/\.(tgs|json|png)$/i, '')    // убираем расширение
        .replace(/^(models?|patterns?)_/, '')  // убираем известные префиксы
        .replace(/[\s-]+/g, '_')              // пробелы и дефисы -> подчёркивание
        .trim();
}

// Ищет в ZIP запись .tgs, устойчиво сопоставляя имена
function findZipEntryByName(zipEntries, targetName) {
    if (!targetName) return { entry: null, reason: 'empty targetName' };
    const normTarget = normalizeName(targetName);

    const tgsEntries = zipEntries.filter(e => e.entryName.toLowerCase().endsWith('.tgs'));

    // Заранее считаем кандидатов с нормализованными именами файлов
    const candidates = tgsEntries.map(e => ({
        entry: e,
        base: e.entryName.replace(/\\+/g,'/').split('/').pop(),
    })).map(x => ({ ...x, norm: normalizeName(x.base) }));

    // 1) точное совпадение нормализованных имён
    let found = candidates.find(c => c.norm === normTarget);
    if (found) return { entry: found.entry, reason: 'norm exact' };

    // 2) совпадение по началу строки (допускает суффиксы вроде _30)
    found = candidates.find(c => c.norm.startsWith(normTarget));
    if (found) return { entry: found.entry, reason: 'norm startsWith' };

    // 3) ослабленное: отбрасываем хвост из _цифр у кандидата и сравниваем
    found = candidates.find(c => c.norm.replace(/_[0-9]+$/, '') === normTarget);
    if (found) return { entry: found.entry, reason: 'strip _digits equals' };

    // 4) совпадение по началу после отбрасывания цифрового хвоста
    found = candidates.find(c => c.norm.replace(/_[0-9]+$/, '').startsWith(normTarget));
    if (found) return { entry: found.entry, reason: 'strip _digits startsWith' };

    // 5) убираем подчёркивания у целевого имени и сравниваем по началу
    const targetTight = normTarget.replace(/_/g, '');
    found = candidates.find(c => c.norm.replace(/_/g,'').startsWith(targetTight));
    if (found) return { entry: found.entry, reason: 'tight startsWith' };

    return { entry: null, reason: `no match for ${normTarget}` };
}

const toJSON = (data) => {
    return JSON.parse(JSON.stringify(data, (key, value) => {
        if (typeof value === 'bigint') return value.toString();
        return value;
    }));
};

router.get('/', async (req, res) => {
    try {
        const attributeSets = await req.db.collection('StarGiftAttributeSets').find({}).sort({ _id: -1 }).toArray();
        res.json(toJSON(attributeSets));
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

router.post('/', upload.fields([
    { name: 'model', maxCount: 1 },
    { name: 'pattern', maxCount: 1 }
]), async (req, res) => {
    try {
        const { 
            giftId, 
            name, 
            modelName, 
            patternName, 
            modelRarity, 
            patternRarity
        } = req.body;

        if (!giftId) return res.status(400).json({ error: 'Gift ID is required' });
        if (!name) return res.status(400).json({ error: 'Name is required' });

        const attributeSet = {
            _id: Date.now().toString(),
            GiftId: parseInt(giftId),
            Name: name,
            ModelName: modelName || name + ' Model',
            PatternName: patternName || name + ' Pattern',
            Model: null,
            Pattern: null,
            CreatedAt: new Date()
        };

        // Модель (TGS)
        if (req.files.model && req.files.model[0]) {
            const modelFile = req.files.model[0];
            const documentId = generateDocumentId();
            const accessHash = generateAccessHash();
            const fileReference = generateFileReference();
            const fileBuffer = await fs.readFile(modelFile.path);

            try {
                const decompressed = await gunzip(fileBuffer);
                JSON.parse(decompressed.toString('utf8'));
            } catch (error) {
                try {
                    JSON.parse(fileBuffer.toString('utf8'));
                } catch (jsonError) {
                    await fs.unlink(modelFile.path);
                    return res.status(400).json({ error: 'Invalid TGS file for model' });
                }
            }

            await minioHelper.uploadFile(modelFile.path, documentId, 'application/x-tgsticker');

            // Сохраняем DocumentId и AccessHash как объекты Long (формат MongoDB)
            const documentReadModel = {
                _id: `document-${documentId}`,
                DocumentId: documentId,
                AccessHash: BigInt(accessHash),
                FileReference: fileReference,
                DcId: 2,
                Date: Math.floor(Date.now() / 1000),
                MimeType: 'application/x-tgsticker',
                Size: fileBuffer.length,
                Name: modelFile.originalname,
                Md5CheckSum: null,
                CreatorId: null,
                Thumbs: null,
                ThumbId: null,
                VideoThumbs: null,
                VideoThumbId: null,
                Version: 1,
                Fingerprint: null,
                Attributes: null,
                Attributes2: [
                    { _t: 'TDocumentAttributeFilename', FileName: modelFile.originalname },
                    { _t: 'TDocumentAttributeImageSize', W: 512, H: 512 },
                    { _t: 'TDocumentAttributeAnimated' },
                    { _t: 'TDocumentAttributeSticker', Mask: false, Alt: '⭐️', Stickerset: { _t: 'TInputStickerSetEmpty' }, MaskCoords: null }
                ]
            };

            await req.db.collection('ReadModel-DocumentReadModel').insertOne(documentReadModel);
            await fs.unlink(modelFile.path);
            attributeSet.Model = { DocumentId: documentId, RarityPermille: parseInt(modelRarity) || 1000 };
            console.log(`Model uploaded: DocumentId=${documentId}, AccessHash=${accessHash}`);
        }

        // Узор (PNG или TGS)
        if (req.files.pattern && req.files.pattern[0]) {
            const patternFile = req.files.pattern[0];
            const documentId = generateDocumentId();
            const accessHash = generateAccessHash();
            const fileReference = generateFileReference();
            const ext = path.extname(patternFile.originalname).toLowerCase();
            let mimeType = 'image/png';
            let attributes = [
                { _t: 'TDocumentAttributeFilename', FileName: patternFile.originalname },
                { _t: 'TDocumentAttributeImageSize', W: 512, H: 512 }
            ];

            const fileBuffer = await fs.readFile(patternFile.path);

            if (ext === '.tgs') {
                mimeType = 'application/x-tgsticker';
                try {
                    const decompressed = await gunzip(fileBuffer);
                    JSON.parse(decompressed.toString('utf8'));
                } catch (error) {
                    try {
                        JSON.parse(fileBuffer.toString('utf8'));
                    } catch (jsonError) {
                        await fs.unlink(patternFile.path);
                        return res.status(400).json({ error: 'Invalid TGS file for pattern' });
                    }
                }
                attributes.push({ _t: 'TDocumentAttributeAnimated' });
                attributes.push({ _t: 'TDocumentAttributeSticker', Alt: '⭐️', Stickerset: { _t: 'TInputStickerSetEmpty' }, Mask: false });
            }

            await minioHelper.uploadFile(patternFile.path, documentId, mimeType);

            const documentReadModel = {
                _id: `document-${documentId}`,
                DocumentId: documentId,
                AccessHash: BigInt(accessHash),
                FileReference: fileReference,
                DcId: 2,
                Date: Math.floor(Date.now() / 1000),
                MimeType: mimeType,
                Size: fileBuffer.length,
                Name: patternFile.originalname,
                Md5CheckSum: null,
                CreatorId: null,
                Thumbs: null,
                ThumbId: null,
                VideoThumbs: null,
                VideoThumbId: null,
                Version: 1,
                Fingerprint: null,
                Attributes: null,
                Attributes2: attributes
            };

            await req.db.collection('ReadModel-DocumentReadModel').insertOne(documentReadModel);
            await fs.unlink(patternFile.path);
            attributeSet.Pattern = { DocumentId: documentId, RarityPermille: parseInt(patternRarity) || 1000 };
        }

        // Фон генерируется на сервере автоматически из 100 цветовых схем,
        // загружать файлы фона или задавать цвета вручную не требуется

        await req.db.collection('StarGiftAttributeSets').insertOne(attributeSet);
        res.status(201).json(toJSON(attributeSet));
    } catch (error) {
        console.error('Create attribute set error:', error);
        if (req.files) {
            const allFiles = [...(req.files.model || []), ...(req.files.pattern || [])];
            for (const file of allFiles) {
                await fs.unlink(file.path).catch(() => { });
            }
        }
        res.status(500).json({ error: error.message });
    }
});

router.delete('/:id', async (req, res) => {
    try {
        const setId = req.params.id;
        console.log(`[DELETE] Attempting to delete attribute set with ID: ${setId}`);

        // Сначала пробуем найти по _id как по строке
        let attributeSet = await req.db.collection('StarGiftAttributeSets').findOne({ _id: setId });

        // Если не нашли, а ID похож на ObjectId, пробуем как ObjectId
        if (!attributeSet && setId.match(/^[0-9a-fA-F]{24}$/)) {
            console.log(`Trying with ObjectId format...`);
            attributeSet = await req.db.collection('StarGiftAttributeSets').findOne({ _id: new ObjectId(setId) });
        }

        if (!attributeSet) {
            console.log(`Attribute set not found with ID: ${setId}`);
            return res.status(404).json({ error: 'Attribute set not found' });
        }

        console.log(`Found attribute set: ${attributeSet.Name}`);

        const documentIds = [];
        if (attributeSet.Model) documentIds.push(attributeSet.Model.DocumentId);
        if (attributeSet.Pattern) documentIds.push(attributeSet.Pattern.DocumentId);
        // Фон генерируется автоматически, удалять документ не нужно

        for (const docId of documentIds) {
            await req.db.collection('ReadModel-DocumentReadModel').deleteOne({ DocumentId: docId });
            try {
                await minioHelper.deleteFile(docId);
            } catch (err) {
                console.warn(`Failed to delete file ${docId} from MinIO:`, err);
            }
        }

        // Удаляем в том же формате _id, в каком запись была найдена
        await req.db.collection('StarGiftAttributeSets').deleteOne({ _id: attributeSet._id });
        console.log(`Successfully deleted attribute set: ${attributeSet.Name}`);
        res.json({ message: 'Attribute set deleted successfully' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// POST /attributes/upload-zip - загрузка ZIP-архива с улучшениями подарка
router.post('/upload-zip', uploadZip.single('file'), async (req, res) => {
    try {
        if (!req.file) {
            return res.status(400).json({ error: 'No file uploaded' });
        }

        const { giftId } = req.body;
        if (!giftId) {
            return res.status(400).json({ error: 'giftId is required' });
        }

        console.log(`Processing ZIP upload for gift ${giftId}: ${req.file.originalname}`);

        const AdmZip = (await import('adm-zip')).default;
        const zip = new AdmZip(req.file.path);
        const zipEntries = zip.getEntries();

        // Ищем info.json
        const infoEntry = zipEntries.find(entry => entry.entryName === 'info.json' || entry.entryName.endsWith('/info.json'));
        if (!infoEntry) {
            return res.status(400).json({ error: 'info.json not found in ZIP archive' });
        }

        // Разбираем info.json
        const infoContent = infoEntry.getData().toString('utf8');
        const info = JSON.parse(infoContent);
        
        const giftName = info.gift_name || `Gift ${giftId}`;
        const models = info.models || [];
        const patterns = info.patterns || [];
        const backdrops = info.backdrops || [];
        
        console.log(`Found info.json: gift="${giftName}", models=${models.length}, patterns=${patterns.length}, backdrops=${backdrops.length}`);

        if (models.length === 0 || patterns.length === 0) {
            return res.status(400).json({ error: 'No models or patterns found in info.json' });
        }

        const results = [];
        const db = req.db;

        // Формируем комбинации: каждая модель с каждым узором и случайным фоном
        const maxCombinations = Math.min(models.length * patterns.length, 100); // не более 100 комбинаций
        console.log(`Creating ${maxCombinations} attribute combinations...`);

        for (let i = 0; i < Math.min(models.length, 20); i++) {
            const model = models[i];
            for (let j = 0; j < Math.min(patterns.length, 5); j++) {
                if (results.length >= 50) break; // общее ограничение

                const pattern = patterns[j];
                const backdrop = backdrops[Math.floor(Math.random() * backdrops.length)];

                console.log(`\nProcessing combination ${results.length + 1}: ${model.name} + ${pattern.name}`);

                let modelDocId = null;
                let patternDocId = null;

                // Загружаем файл модели
                if (model.file) {
                    const { entry: modelEntry, reason } = findZipEntryByName(zipEntries, model.file);
                    if (modelEntry) {
                        const modelBuffer = modelEntry.getData();
                        modelDocId = await uploadDocument(db, modelBuffer, model.file, 'model');
                        console.log(`  Model uploaded: ${model.file} -> ${modelDocId} (${reason})`);
                    } else {
                        console.log(`  Model file not found: ${model.file} (${reason})`);
                        continue; // пропускаем эту комбинацию
                    }
                }

                // Загружаем файл узора
                if (pattern.file) {
                    const { entry: patternEntry, reason } = findZipEntryByName(zipEntries, pattern.file);
                    if (patternEntry) {
                        const patternBuffer = patternEntry.getData();
                        patternDocId = await uploadDocument(db, patternBuffer, pattern.file, 'pattern');
                        console.log(`  Pattern uploaded: ${pattern.file} -> ${patternDocId} (${reason})`);
                    } else {
                        console.log(`  Pattern file not found: ${pattern.file} (${reason})`);
                        continue; // пропускаем эту комбинацию
                    }
                }

                // Создаём запись набора атрибутов
                const attributeSet = {
                    _id: Date.now().toString() + '-' + Math.random().toString(36).substring(7), // генерируем уникальный ID
                    GiftId: parseInt(giftId),
                    Name: `${model.name} ${pattern.name}`,
                    Model: modelDocId ? {
                        DocumentId: modelDocId,
                        RarityPermille: Math.round(model.rarity_percent * 10) // переводим проценты в промилле
                    } : null,
                    Pattern: patternDocId ? {
                        DocumentId: patternDocId,
                        RarityPermille: Math.round(pattern.rarity_percent * 10)
                    } : null,
                    Backdrop: backdrop ? {
                        BackdropId: backdrop.id || parseInt(giftId),
                        CenterColor: parseInt(backdrop.center_color?.replace('#', '') || 'FF0000', 16),
                        EdgeColor: parseInt(backdrop.edge_color?.replace('#', '') || '0000FF', 16),
                        PatternColor: parseInt(backdrop.pattern_color?.replace('#', '') || '5AC8FA', 16),
                        TextColor: parseInt(backdrop.text_color?.replace('#', '') || 'FFFFFF', 16),
                        RarityPermille: Math.round(backdrop.rarity_percent * 10)
                    } : null,
                    ModelName: model.name,
                    PatternName: pattern.name,
                    BackdropName: backdrop?.name || 'Random',
                    CreatedAt: new Date() // отметка времени для единообразия
                };

                // Добавляем запись в StarGiftAttributeSets
                await db.collection('StarGiftAttributeSets').insertOne(attributeSet);
                console.log(`  Attribute set created: ${model.name} + ${pattern.name}`);

                results.push({
                    model: model.name,
                    pattern: pattern.name,
                    backdrop: backdrop?.name,
                    modelDocId,
                    patternDocId
                });
            }
            if (results.length >= 50) break;
        }

        // Удаляем загруженный ZIP-файл
        await fs.unlink(req.file.path);

        console.log(`\nSuccessfully processed ${results.length} upgrades for gift ${giftId}`);

        res.json({
            success: true,
            giftId: parseInt(giftId),
            title: giftName,
            upgradesProcessed: results.length,
            results
        });

    } catch (error) {
        console.error('ZIP upload error:', error);
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// Вспомогательная функция: сохраняет документ в MongoDB
async function uploadDocument(db, fileBuffer, filename, type) {
    const documentId = Date.now() * 1000 + Math.floor(Math.random() * 1000);
    
    const document = {
        _id: `document-${documentId}`,
        Id: `document-${documentId}`,
        DocumentId: documentId,
        AccessHash: generateAccessHash(),
        FileReference: generateFileReference(),
        Date: Math.floor(Date.now() / 1000),
        MimeType: 'application/x-tgsticker',
        Size: fileBuffer.length,
        DcId: 2,
        // Attributes2 оставляем пустым: сервер ждёт TL-типы, а не сырой JSON
        Attributes2: [],
        // Attributes не задаём массивом — опускаем или ставим null, сервер ждёт byte[].
        // Здесь поле опущено, чтобы соответствовать схеме
        Thumbs: [],
        VideoThumbs: []
    };

    // Пишем в обе коллекции, чтобы query-server увидел документ
    await db.collection('eventflow-documentreadmodel').insertOne(document);
    await db.collection('ReadModel-DocumentReadModel').insertOne(document);

    // Заливаем содержимое в MinIO под именем объекта = DocumentId (без расширения).
    // Сохраняем буфер во временный файл, загружаем и удаляем его
    const tmpDir = 'uploads/tmp';
    await fs.mkdir(tmpDir, { recursive: true });
    const tmpPath = `${tmpDir}/doc_${documentId}.bin`;
    await fs.writeFile(tmpPath, fileBuffer);
    try {
        // отложенный импорт, чтобы избежать циклической зависимости
        const { uploadFile } = (await import('../utils/minioHelper.js')).default ? (await import('../utils/minioHelper.js')).default : await import('../utils/minioHelper.js');
        // minioHelper экспортирует default с методами; поддерживаем и default, и именованный экспорт
        const helper = (await import('../utils/minioHelper.js')).default;
        if (helper && helper.uploadFile) {
            await helper.uploadFile(tmpPath, documentId, 'application/x-tgsticker');
        } else if (uploadFile) {
            await uploadFile(tmpPath, documentId, 'application/x-tgsticker');
        }
    } finally {
        try { await fs.unlink(tmpPath); } catch {}
    }
    
    return documentId;
}

export default router;
