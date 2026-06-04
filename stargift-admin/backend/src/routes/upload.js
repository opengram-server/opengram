import express from 'express';
import multer from 'multer';
import path from 'path';
import fs from 'fs';

const router = express.Router();

// Настраиваем multer для загрузки файлов
const upload = multer({
  dest: 'uploads/',
  limits: {
    fileSize: 10 * 1024 * 1024 // ограничение 10 МБ
  },
  fileFilter: (req, file, cb) => {
    // Разрешаем JSON-файлы для анимаций
    if (file.mimetype === 'application/json' || file.originalname.endsWith('.json')) {
      cb(null, true);
    } else {
      cb(new Error('Only JSON files are allowed for animations'));
    }
  }
});

// Загрузить документ в Telegram
router.post('/document', upload.single('file'), async (req, res) => {
  try {
    if (!req.file) {
      return res.status(400).json({ error: 'No file uploaded' });
    }
    
    const { type, title } = req.body;
    const filePath = req.file.path;
    const originalName = req.file.originalname;
    
    console.log(`Uploading document: ${originalName}, type: ${type}, title: ${title}`);

    try {
      // Читаем содержимое файла
      const fileBuffer = fs.readFileSync(filePath);

      // Для JSON-анимаций проверяем содержимое
      if (type === 'animation' && originalName.endsWith('.json')) {
        try {
          const jsonContent = JSON.parse(fileBuffer.toString());
          if (!jsonContent.v || !jsonContent.layers) {
            throw new Error('Invalid Lottie JSON: missing required fields (v, layers)');
          }
          console.log(`Valid Lottie JSON: version=${jsonContent.v}, layers=${jsonContent.layers?.length}`);
        } catch (jsonError) {
          return res.status(400).json({ 
            success: false, 
            error: 'Invalid JSON animation: ' + jsonError.message 
          });
        }
      }
      
      // Генерируем ID документа (по метке времени, как у остальных документов)
      const documentId = Date.now() * 1000 + Math.floor(Math.random() * 1000);

      // Создаём документ в MongoDB
      const document = {
        _id: `document-${documentId}`,
        Id: `document-${documentId}`,
        DocumentId: documentId,
        AccessHash: Math.floor(Math.random() * 9223372036854775807), // случайное 64-битное значение
        FileReference: Buffer.from([0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08]), // 8 байт
        Date: Math.floor(Date.now() / 1000),
        MimeType: type === 'animation' ? 'application/json' : 'application/octet-stream',
        Size: fileBuffer.length,
        DcId: 2, // дата-центр для медиа
        Attributes2: [
          {
            ConstructorId: 0x6319d612, // TDocumentAttributeSticker
            Alt: type === 'animation' ? '❄️' : '📄',
            Stickerset: {
              ConstructorId: 0x83cf7966, // TInputStickerSetEmpty
            }
          }
        ],
        Thumbs: [],
        VideoThumbs: []
      };
      
      // Сохраняем документ в MongoDB
      await req.db
        .collection('eventflow-documentreadmodel')
        .insertOne(document);

      // TODO: загрузить файл в хранилище MinIO.
      // Пока создаём только запись документа.
      // В продакшене сюда нужно заливать fileBuffer в MinIO по пути `${documentId}`.

      console.log(`Document created: ID=${documentId}, Size=${fileBuffer.length} bytes`);
      
      res.json({
        success: true,
        documentId: documentId,
        message: `Document uploaded successfully`,
        size: fileBuffer.length
      });
      
    } finally {
      // Удаляем загруженный файл
      if (fs.existsSync(filePath)) {
        fs.unlinkSync(filePath);
      }
    }
    
  } catch (error) {
    console.error('Upload document error:', error);
    res.status(500).json({ 
      success: false, 
      error: error.message 
    });
  }
});

export default router;
