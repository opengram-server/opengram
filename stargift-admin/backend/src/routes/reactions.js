import express from 'express';
import multer from 'multer';
import { MongoClient, ObjectId } from 'mongodb';
import AdmZip from 'adm-zip';
import pako from 'pako';
import * as Minio from 'minio';

const router = express.Router();

// Клиент MinIO
const minioClient = new Minio.Client({
  endPoint: process.env.MINIO_ENDPOINT || 'localhost',
  port: parseInt(process.env.MINIO_PORT || '9000'),
  useSSL: process.env.MINIO_USE_SSL === 'true',
  accessKey: process.env.MINIO_ACCESS_KEY || 'test',
  secretKey: process.env.MINIO_SECRET_KEY || '12345678'
});

const BUCKET_NAME = process.env.MINIO_BUCKET || 'tg-files';

const upload = multer({
  storage: multer.memoryStorage(),
  limits: { fileSize: 100 * 1024 * 1024 } // 100MB
});

const MONGO_URL = process.env.MONGODB_URI || 'mongodb://localhost:27017';
const DB_NAME = process.env.DB_NAME || 'tg';

// Список всех реакций
router.get('/', async (req, res) => {
  const client = new MongoClient(MONGO_URL);

  try {
    await client.connect();
    const db = client.db(DB_NAME);
    const collection = db.collection('reactions');

    const reactions = await collection.find({}).toArray();

    res.json({
      success: true,
      reactions: reactions.map(r => ({
        id: r._id.toString(),
        emoji: r.emoji,
        title: r.title,
        premium: r.premium || false,
        inactive: r.inactive || false,
        hasAllAnimations: !!(r.staticIcon && r.appearAnimation && r.selectAnimation && r.activateAnimation && r.effectAnimation),
        selectAnimation: r.selectAnimationPreview
      }))
    });
  } catch (error) {
    console.error('Error fetching reactions:', error);
    res.status(500).json({ error: 'Failed to fetch reactions' });
  } finally {
    await client.close();
  }
});

// Создать реакцию
router.post('/', upload.fields([
  { name: 'staticIcon', maxCount: 1 },
  { name: 'appearAnimation', maxCount: 1 },
  { name: 'selectAnimation', maxCount: 1 },
  { name: 'activateAnimation', maxCount: 1 },
  { name: 'effectAnimation', maxCount: 1 },
  { name: 'aroundAnimation', maxCount: 1 },
  { name: 'centerIcon', maxCount: 1 }
]), async (req, res) => {
  const client = new MongoClient(MONGO_URL);

  try {
    const data = JSON.parse(req.body.data);
    await client.connect();
    const db = client.db(DB_NAME);

    // Загружаем анимации в MinIO и создаём документы
    const animations = {};
    const animationFields = [
      'staticIcon', 'appearAnimation', 'selectAnimation',
      'activateAnimation', 'effectAnimation', 'aroundAnimation', 'centerIcon'
    ];

    for (const field of animationFields) {
      if (req.files[field] && req.files[field][0]) {
        const file = req.files[field][0];
        const documentId = Date.now() + Math.floor(Math.random() * 1000000);

        // Загружаем в MinIO (file-server ожидает файлы в корне: {documentId})
        await minioClient.putObject(BUCKET_NAME, `${documentId}`, file.buffer, file.size, {
          'Content-Type': 'application/x-tgsticker'
        });

        // Создаём документ в MongoDB
        const fileReference = Buffer.from([
          0x01, 0x00, 0x00, 0x00,
          (documentId >> 24) & 0xFF,
          (documentId >> 16) & 0xFF,
          (documentId >> 8) & 0xFF,
          documentId & 0xFF
        ]);

        const documentReadModelId = `document-${documentId}`;
        const document = {
          _id: documentReadModelId,
          Id: documentReadModelId,
          Version: 1,
          DocumentId: documentId,
          AccessHash: documentId,
          FileReference: fileReference,
          Date: Math.floor(Date.now() / 1000),
          MimeType: 'application/x-tgsticker',
          Size: file.size,
          Name: `${documentId}`,
          DcId: 2,
          Md5CheckSum: null,
          CreatorId: null,
          Thumbs: null,
          ThumbId: null,
          VideoThumbs: null,
          VideoThumbId: null,
          Attributes: null,
          Attributes2: [{
            _t: 'TDocumentAttributeSticker',
            Alt: data.emoji || '😀',
            Stickerset: {
              _t: 'TInputStickerSetEmpty'
            }
          }]
        };

        await db.collection('ReadModel-DocumentReadModel').insertOne(document);
        animations[field] = documentId;

        // Сохраняем превью для анимации выбора
        if (field === 'selectAnimation') {
          try {
            const decompressed = pako.inflate(file.buffer, { to: 'string' });
            animations.selectAnimationPreview = JSON.parse(decompressed);
          } catch (e) {
            console.log('Could not create preview:', e.message);
          }
        }
      }
    }

    // Создаём реакцию
    const reaction = {
      emoji: data.emoji,
      title: data.title,
      premium: data.premium || false,
      inactive: data.inactive || false,
      ...animations,
      createdAt: new Date()
    };

    const result = await db.collection('reactions').insertOne(reaction);

    res.json({
      success: true,
      reactionId: result.insertedId.toString()
    });
  } catch (error) {
    console.error('Error creating reaction:', error);
    res.status(500).json({ error: 'Failed to create reaction' });
  } finally {
    await client.close();
  }
});

// Массовая загрузка из ZIP (по папкам: emoji/asset.ext)
router.post('/bulk-upload', upload.single('zipFile'), async (req, res) => {
  const client = new MongoClient(MONGO_URL);

  try {
    if (!req.file) {
      return res.status(400).json({ error: 'No ZIP file provided' });
    }

    const premium = req.body.premium === 'true';
    const zip = new AdmZip(req.file.buffer);
    const zipEntries = zip.getEntries();

    await client.connect();
    const db = client.db(DB_NAME);

    const results = [];
    const reactionsMap = new Map(); // Map<emoji, { assets: {}, title: string }>

    // 1. Группируем файлы по эмодзи (имени папки)
    for (const entry of zipEntries) {
      if (entry.isDirectory || entry.entryName.startsWith('__MACOSX')) continue;

      // Приводим разделители пути к единому виду
      const entryPath = entry.entryName.replace(/\\/g, '/');
      const parts = entryPath.split('/');

      // Ожидаем как минимум папку и файл: folder/file.ext
      if (parts.length < 2) continue;

      // Папка с эмодзи — это непосредственный родитель файла
      const emojiFolder = parts[parts.length - 2];
      const filename = parts[parts.length - 1];
      const nameWithoutExt = filename.split('.')[0];

      // Проверяем тип ассета
      const validAssets = [
        'static_icon', 'appear_animation', 'select_animation',
        'activate_animation', 'effect_animation', 'around_animation', 'center_icon'
      ];

      if (!validAssets.includes(nameWithoutExt)) continue;

      if (!reactionsMap.has(emojiFolder)) {
        reactionsMap.set(emojiFolder, {
          emoji: emojiFolder,
          assets: {}
        });
      }

      const reactionData = reactionsMap.get(emojiFolder);
      reactionData.assets[nameWithoutExt] = {
        buffer: entry.getData(),
        filename: filename
      };
    }

    // 2. Обрабатываем каждую реакцию
    for (const [emoji, data] of reactionsMap) {
      try {
        console.log(`Processing reaction: ${emoji}`);
        const animationIds = {};

        // Загружаем ассеты
        for (const [assetType, fileData] of Object.entries(data.assets)) {
          const documentId = Date.now() + Math.floor(Math.random() * 1000000);

          // Определяем MIME-тип по расширению
          const ext = fileData.filename.split('.').pop().toLowerCase();
          let mimeType = 'application/x-tgsticker'; // по умолчанию для TGS
          if (ext === 'webm') mimeType = 'video/webm';
          if (ext === 'webp') mimeType = 'image/webp';
          if (ext === 'png') mimeType = 'image/png';

          // Загружаем в MinIO
          await minioClient.putObject(BUCKET_NAME, `${documentId}`, fileData.buffer, fileData.buffer.length, {
            'Content-Type': mimeType
          });

          // Создаём DocumentReadModel
          const fileReference = Buffer.from([
            0x01, 0x00, 0x00, 0x00,
            (documentId >> 24) & 0xFF,
            (documentId >> 16) & 0xFF,
            (documentId >> 8) & 0xFF,
            documentId & 0xFF
          ]);

          const documentReadModelId = `document-${documentId}`;
          const document = {
            _id: documentReadModelId,
            Id: documentReadModelId,
            Version: 1,
            DocumentId: documentId,
            AccessHash: documentId,
            FileReference: fileReference,
            Date: Math.floor(Date.now() / 1000),
            MimeType: mimeType,
            Size: fileData.buffer.length,
            Name: `${documentId}`,
            DcId: 2,
            Md5CheckSum: null,
            CreatorId: null,
            Thumbs: null,
            ThumbId: null,
            VideoThumbs: null,
            VideoThumbId: null,
            Attributes: null,
            Attributes2: [{
              _t: 'TDocumentAttributeSticker',
              Alt: emoji,
              Stickerset: {
                _t: 'TInputStickerSetEmpty'
              }
            }]
          };

          await db.collection('ReadModel-DocumentReadModel').insertOne(document);
          animationIds[assetType] = documentId;

          // Сохраняем превью для анимации выбора
          if (assetType === 'selectAnimation' && mimeType === 'application/x-tgsticker') {
            try {
              const decompressed = pako.inflate(fileData.buffer, { to: 'string' });
              animationIds.selectAnimationPreview = JSON.parse(decompressed);
            } catch (e) {
              console.log(`Could not create preview for ${emoji}:`, e.message);
            }
          }
        }

        // Создаём или обновляем реакцию
        const reaction = {
          emoji: emoji,
          title: emoji,
          premium: premium,
          inactive: false,
          ...animationIds,
          createdAt: new Date()
        };

        // Проверяем, есть ли уже такая реакция
        const existing = await db.collection('reactions').findOne({ emoji: emoji });
        if (existing) {
          await db.collection('reactions').updateOne({ _id: existing._id }, { $set: reaction });
        } else {
          await db.collection('reactions').insertOne(reaction);
        }

        results.push({
          success: true,
          emoji: emoji,
          assets: Object.keys(animationIds)
        });

      } catch (error) {
        console.error(`Error processing ${emoji}:`, error);
        results.push({
          success: false,
          emoji: emoji,
          error: error.message
        });
      }
    }

    res.json({
      success: true,
      results
    });
  } catch (error) {
    console.error('Error bulk uploading reactions:', error);
    res.status(500).json({ error: 'Failed to process ZIP file' });
  } finally {
    await client.close();
  }
});

// Удалить реакцию
router.delete('/:id', async (req, res) => {
  const client = new MongoClient(MONGO_URL);

  try {
    await client.connect();
    const db = client.db(DB_NAME);

    const reaction = await db.collection('reactions').findOne({ _id: new ObjectId(req.params.id) });

    if (!reaction) {
      return res.status(404).json({ error: 'Reaction not found' });
    }

    // Удаляем документы анимаций из MinIO и MongoDB
    const animationFields = [
      'staticIcon', 'appearAnimation', 'selectAnimation',
      'activateAnimation', 'effectAnimation', 'aroundAnimation', 'centerIcon'
    ];

    for (const field of animationFields) {
      if (reaction[field]) {
        try {
          await minioClient.removeObject(BUCKET_NAME, `${reaction[field]}`);
          await db.collection('eventflow-documentreadmodel').deleteOne({ DocumentId: reaction[field] });
        } catch (e) {
          console.log(`Failed to delete ${field}:`, e.message);
        }
      }
    }

    // Удаляем реакцию
    await db.collection('reactions').deleteOne({ _id: new ObjectId(req.params.id) });

    res.json({ success: true });
  } catch (error) {
    console.error('Error deleting reaction:', error);
    res.status(500).json({ error: 'Failed to delete reaction' });
  } finally {
    await client.close();
  }
});

export default router;
