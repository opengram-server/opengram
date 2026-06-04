import express from 'express';
import multer from 'multer';
import path from 'path';
import { fileURLToPath } from 'url';
import fs from 'fs/promises';
import zlib from 'zlib';
import { promisify } from 'util';
import minioHelper from '../utils/minioHelper.js';

const gunzip = promisify(zlib.gunzip);
const router = express.Router();

// Настройка загрузки файлов TGS
const storage = multer.diskStorage({
  destination: async (req, file, cb) => {
    const uploadDir = path.join(process.cwd(), 'uploads', 'emoji-packs');
    await fs.mkdir(uploadDir, { recursive: true });
    cb(null, uploadDir);
  },
  filename: (req, file, cb) => {
    const uniqueSuffix = Date.now() + '-' + Math.round(Math.random() * 1E9);
    cb(null, 'emoji-' + uniqueSuffix + path.extname(file.originalname));
  }
});

const upload = multer({
  storage,
  limits: { fileSize: 100 * 1024 * 1024 }, // 100 MB max (для ZIP архивов)
  fileFilter: (req, file, cb) => {
    const allowedTypes = ['.tgs', '.json', '.webm', '.zip', '.webp', '.png'];
    const ext = path.extname(file.originalname).toLowerCase();

    if (allowedTypes.includes(ext)) {
      cb(null, true);
    } else {
      cb(new Error('Only TGS, JSON, WEBM, ZIP, WEBP, and PNG files are allowed'));
    }
  }
});

// Хранение в памяти для импорта метаданных (быстрее, без обращений к диску)
const uploadMemory = multer({
  storage: multer.memoryStorage(),
  limits: { fileSize: 100 * 1024 * 1024 }
});

// Список всех эмодзи-паков
router.get('/', async (req, res) => {
  try {
    const { page = 1, limit = 20, search = '', emojis = '' } = req.query;
    const skip = (parseInt(page) - 1) * parseInt(limit);

    const filter = {};
    if (search) {
      filter.$or = [
        { Title: { $regex: search, $options: 'i' } },
        { ShortName: { $regex: search, $options: 'i' } }
      ];
    }
    if (emojis) {
      filter.Emojis = true;
    }

    console.log('Emoji packs filter:', JSON.stringify(filter));

    const packs = await req.db.collection('ReadModel-StickerSetReadModel')
      .find(filter)
      .sort({ StickerSetId: -1 })
      .skip((page - 1) * limit)
      .limit(parseInt(limit))
      .toArray();

    console.log(`Found ${packs.length} emoji packs (emojis param: ${emojis})`);

    const total = await req.db.collection('ReadModel-StickerSetReadModel')
      .countDocuments(filter);

    // Считаем количество эмодзи в каждом паке
    const packsWithCounts = await Promise.all(
      packs.map(async (pack) => {
        const emojiCount = await req.db.collection('custom_emoji_documents')
          .countDocuments({ 'attributes.stickerset_id': pack.StickerSetId });

        return { ...pack, emoji_count: emojiCount };
      })
    );

    res.json({
      success: true,
      packs: packsWithCounts,
      pagination: {
        page: parseInt(page),
        limit: parseInt(limit),
        total,
        totalPages: Math.ceil(total / parseInt(limit))
      }
    });
  } catch (error) {
    console.error('Error fetching emoji packs:', error);
    res.status(500).json({ error: error.message });
  }
});

// ============================================
// УПРАВЛЕНИЕ РЕКОМЕНДУЕМЫМИ ЭМОДЗИ-ПАКАМИ
// ============================================

// Список рекомендуемых эмодзи-паков
router.get('/featured', async (req, res) => {
  try {
    console.log('Fetching featured emoji packs...');

    const featuredPacks = await req.db.collection('ReadModel-StickerSetReadModel')
      .find({ IsFeatured: true, Emojis: true })
      .sort({ FeaturedOrder: 1 })
      .toArray();

    console.log(`Found ${featuredPacks.length} featured emoji packs`);

    res.json({
      success: true,
      count: featuredPacks.length,
      packs: featuredPacks
    });
  } catch (error) {
    console.error('Error fetching featured packs:', error);
    res.status(500).json({ error: error.message });
  }
});

// Назначить пак рекомендуемым
router.post('/:stickerSetId/featured', async (req, res) => {
  try {
    const { stickerSetId } = req.params;
    const { isFeatured = true, featuredOrder = 0 } = req.body;

    console.log(`Setting featured status for pack ${stickerSetId}: ${isFeatured}, order: ${featuredOrder}`);

    const result = await req.db.collection('ReadModel-StickerSetReadModel').updateOne(
      { StickerSetId: parseInt(stickerSetId) },
      {
        $set: {
          IsFeatured: isFeatured,
          FeaturedOrder: parseInt(featuredOrder)
        }
      }
    );

    if (result.matchedCount === 0) {
      console.log(`Sticker set ${stickerSetId} not found`);
      return res.status(404).json({ error: 'Sticker set not found' });
    }

    console.log(`Updated featured status: matched=${result.matchedCount}, modified=${result.modifiedCount}`);

    res.json({
      success: true,
      message: `Pack ${isFeatured ? 'added to' : 'removed from'} featured`,
      modified: result.modifiedCount
    });
  } catch (error) {
    console.error('Error updating featured status:', error);
    res.status(500).json({ error: error.message });
  }
});

// Массовое обновление порядка рекомендуемых паков
router.post('/featured/reorder', async (req, res) => {
  try {
    const { packs } = req.body; // массив объектов { stickerSetId, featuredOrder }

    if (!Array.isArray(packs)) {
      return res.status(400).json({ error: 'packs must be an array' });
    }

    console.log(`Reordering ${packs.length} featured packs...`);

    const bulkOps = packs.map(pack => ({
      updateOne: {
        filter: { StickerSetId: parseInt(pack.stickerSetId) },
        update: { $set: { FeaturedOrder: parseInt(pack.featuredOrder) } }
      }
    }));

    const result = await req.db.collection('ReadModel-StickerSetReadModel').bulkWrite(bulkOps);

    console.log(`Reorder complete: modified=${result.modifiedCount}`);

    res.json({
      success: true,
      message: 'Featured packs order updated',
      modified: result.modifiedCount
    });
  } catch (error) {
    console.error('Error reordering featured packs:', error);
    res.status(500).json({ error: error.message });
  }
});

// ============================================
// ЭКСПОРТ И ИМПОРТ ВСЕХ ЭМОДЗИ-ПАКОВ
// ============================================

// Экспорт всех эмодзи-паков в ZIP (JSON + файлы TGS)
router.get('/export', async (req, res) => {
  try {
    console.log('Exporting all emoji packs with files...');

    const archiver = (await import('archiver')).default;
    const { Client } = await import('minio');

    // Инициализируем клиента MinIO
    const minioClient = new Client({
      endPoint: process.env.MINIO_ENDPOINT || 'localhost',
      port: parseInt(process.env.MINIO_PORT || '9000'),
      useSSL: false,
      accessKey: process.env.MINIO_ACCESS_KEY || 'test',
      secretKey: process.env.MINIO_SECRET_KEY || '12345678'
    });
    const MINIO_BUCKET = 'tg-files';

    // Получаем все эмодзи-паки вместе с их документами
    const packs = await req.db.collection('ReadModel-StickerSetReadModel')
      .find({ Emojis: true })
      .toArray();

    console.log(`Found ${packs.length} emoji packs to export`);

    // Для каждого пака получаем все документы
    const packsWithDocuments = await Promise.all(
      packs.map(async (pack) => {
        const documents = await req.db.collection('ReadModel-DocumentReadModel')
          .find({ DocumentId: { $in: pack.StickerDocumentIds || [] } })
          .toArray();

        return {
          pack,
          documents,
          documentCount: documents.length
        };
      })
    );

    const exportData = {
      exportDate: new Date().toISOString(),
      totalPacks: packs.length,
      totalDocuments: packsWithDocuments.reduce((sum, p) => sum + p.documentCount, 0),
      packs: packsWithDocuments
    };

    console.log(`Export complete: ${exportData.totalPacks} packs, ${exportData.totalDocuments} documents`);

    // Создаём ZIP-архив
    const archive = archiver('zip', { zlib: { level: 9 } });

    res.setHeader('Content-Type', 'application/zip');
    res.setHeader('Content-Disposition', `attachment; filename="emoji-packs-backup-${Date.now()}.zip"`);

    archive.pipe(res);

    // Добавляем JSON с метаданными
    archive.append(JSON.stringify(exportData, null, 2), { name: 'metadata.json' });

    // Добавляем все файлы TGS из MinIO
    let filesAdded = 0;
    for (const packData of packsWithDocuments) {
      for (const doc of packData.documents) {
        try {
          const documentId = doc.DocumentId.toString();

          // Забираем файл из MinIO
          const stream = await minioClient.getObject(MINIO_BUCKET, documentId);

          // Кладём в архив с именем по documentId
          archive.append(stream, { name: `files/${documentId}.tgs` });
          filesAdded++;

        } catch (err) {
          console.warn(`Failed to get file for document ${doc.DocumentId}:`, err.message);
        }
      }
    }

    console.log(`Added ${filesAdded} TGS files to archive`);

    await archive.finalize();

  } catch (error) {
    console.error('Export error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Импорт metadata.json (шаг 1)
router.post('/import/metadata', uploadMemory.single('file'), async (req, res) => {
  try {
    if (!req.file) {
      return res.status(400).json({ error: 'No file uploaded' });
    }

    console.log('Importing metadata.json...');

    // Читаем и разбираем метаданные из буфера
    const metadataContent = req.file.buffer.toString('utf-8');
    const metadata = JSON.parse(metadataContent);

    console.log(`Metadata: ${metadata.totalPacks} packs, ${metadata.totalDocuments} documents`);

    let packsImported = 0;
    let documentsImported = 0;
    let packsSkipped = 0;
    let documentsSkipped = 0;

    // Импортируем паки и документы в MongoDB
    for (const packData of metadata.packs) {
      const pack = packData.pack;

      // Проверяем, существует ли пак
      const existingPack = await req.db.collection('ReadModel-StickerSetReadModel')
        .findOne({ StickerSetId: pack.StickerSetId });

      if (existingPack) {
        console.log(`Pack ${pack.Title} already exists, skipping`);
        packsSkipped++;
        documentsSkipped += packData.documents.length;
        continue;
      }

      // Сначала импортируем документы
      for (const doc of packData.documents) {
        const existingDoc = await req.db.collection('ReadModel-DocumentReadModel')
          .findOne({ DocumentId: doc.DocumentId });

        if (existingDoc) {
          documentsSkipped++;
          continue;
        }

        // Преобразуем FileReference из строки base64 в Binary
        if (doc.FileReference && typeof doc.FileReference === 'string') {
          const { Binary } = await import('mongodb');
          const buffer = Buffer.from(doc.FileReference, 'base64');
          doc.FileReference = new Binary(buffer, 0);
        }

        await req.db.collection('ReadModel-DocumentReadModel').insertOne(doc);
        documentsImported++;
      }

      // Вставляем пак со всеми данными (включая StickerDocumentIds из экспорта)
      await req.db.collection('ReadModel-StickerSetReadModel').insertOne(pack);
      packsImported++;
      console.log(`Imported pack: ${pack.Title} with ${pack.StickerDocumentIds?.length || 0} documents`);
    }

    console.log(`Import complete: ${packsImported} packs, ${documentsImported} documents imported`);

    res.json({
      success: true,
      message: 'Metadata imported successfully',
      imported: {
        packs: packsImported,
        documents: documentsImported
      },
      skipped: {
        packs: packsSkipped,
        documents: documentsSkipped
      }
    });

  } catch (error) {
    console.error('Import metadata error:', error);
    console.error('Stack:', error.stack);
    res.status(500).json({ error: error.message, stack: error.stack });
  }
});

// Импорт файлов TGS из ZIP (шаг 2)
router.post('/import/files', uploadMemory.single('file'), async (req, res) => {
  try {
    if (!req.file) {
      return res.status(400).json({ error: 'No file uploaded' });
    }

    console.log(`Importing TGS files from ZIP (${req.file.size} bytes)...`);

    const AdmZip = (await import('adm-zip')).default;
    const { Client } = await import('minio');

    // Инициализируем клиента MinIO
    const minioClient = new Client({
      endPoint: process.env.MINIO_ENDPOINT || 'localhost',
      port: parseInt(process.env.MINIO_PORT || '9000'),
      useSSL: false,
      accessKey: process.env.MINIO_ACCESS_KEY || 'test',
      secretKey: process.env.MINIO_SECRET_KEY || '12345678'
    });
    const MINIO_BUCKET = 'tg-files';

    // Убеждаемся, что bucket существует
    const bucketExists = await minioClient.bucketExists(MINIO_BUCKET);
    if (!bucketExists) {
      await minioClient.makeBucket(MINIO_BUCKET, 'us-east-1');
    }

    console.log('Extracting ZIP...');

    // Распаковываем ZIP
    let zip, zipEntries;
    try {
      zip = new AdmZip(req.file.buffer);
      zipEntries = zip.getEntries();
      console.log(`Found ${zipEntries.length} entries in ZIP`);
    } catch (err) {
      console.error('Failed to extract ZIP:', err);
      return res.status(400).json({ error: 'Invalid ZIP file: ' + err.message });
    }

    let filesUploaded = 0;
    let filesSkipped = 0;

    // Загружаем каждый файл TGS в MinIO
    for (const entry of zipEntries) {
      if (entry.isDirectory || !entry.entryName.endsWith('.tgs')) {
        continue;
      }

      try {
        // Получаем documentId из имени файла (например, "files/1762063044683.tgs" -> "1762063044683")
        const filename = path.basename(entry.entryName);
        const documentId = path.basename(filename, '.tgs');

        // Проверяем, нет ли уже такого файла в MinIO
        try {
          await minioClient.statObject(MINIO_BUCKET, documentId);
          filesSkipped++;
          continue;
        } catch (err) {
          // Файла нет, загружаем его
        }

        // Берём данные файла из ZIP
        const fileData = entry.getData();

        // Загружаем в MinIO
        await minioClient.putObject(
          MINIO_BUCKET,
          documentId,
          fileData,
          fileData.length,
          {
            'Content-Type': 'application/x-tgsticker',
            'document-id': documentId
          }
        );

        filesUploaded++;

      } catch (err) {
        console.error(`Failed to upload ${entry.entryName}:`, err.message);
      }
    }

    console.log(`Files import complete: ${filesUploaded} uploaded, ${filesSkipped} skipped`);

    res.json({
      success: true,
      message: 'Files imported successfully',
      uploaded: filesUploaded,
      skipped: filesSkipped
    });

  } catch (error) {
    console.error('Import files error:', error);
    console.error('Stack:', error.stack);
    res.status(500).json({ error: error.message });
  }
});

// Импорт эмодзи-паков из ZIP (устаревший вариант: всё в одном файле)
router.post('/import', upload.single('file'), async (req, res) => {
  try {
    if (!req.file) {
      return res.status(400).json({ error: 'No file uploaded' });
    }

    console.log('Importing emoji packs from ZIP...');

    const AdmZip = (await import('adm-zip')).default;
    const { Client } = await import('minio');

    // Инициализируем клиента MinIO
    const minioClient = new Client({
      endPoint: process.env.MINIO_ENDPOINT || 'localhost',
      port: parseInt(process.env.MINIO_PORT || '9000'),
      useSSL: false,
      accessKey: process.env.MINIO_ACCESS_KEY || 'test',
      secretKey: process.env.MINIO_SECRET_KEY || '12345678'
    });
    const MINIO_BUCKET = 'tg-files';

    // Распаковываем ZIP
    const zip = new AdmZip(req.file.buffer);
    const zipEntries = zip.getEntries();

    // Ищем metadata.json
    const metadataEntry = zipEntries.find(e => e.entryName === 'metadata.json');
    if (!metadataEntry) {
      return res.status(400).json({ error: 'Invalid backup file: metadata.json not found' });
    }

    const importData = JSON.parse(metadataEntry.getData().toString('utf-8'));

    if (!importData.packs || !Array.isArray(importData.packs)) {
      return res.status(400).json({ error: 'Invalid import file format' });
    }

    console.log(`Import file contains ${importData.totalPacks} packs, ${importData.totalDocuments} documents`);

    let importedPacks = 0;
    let importedDocuments = 0;
    let importedFiles = 0;
    let skippedPacks = 0;
    const errors = [];

    for (const packData of importData.packs) {
      try {
        const { pack, documents } = packData;

        // Проверяем, существует ли пак
        const existingPack = await req.db.collection('ReadModel-StickerSetReadModel')
          .findOne({ StickerSetId: pack.StickerSetId });

        if (existingPack) {
          console.log(`Pack ${pack.Title} already exists, skipping...`);
          skippedPacks++;
          continue;
        }

        // Импортируем пак
        await req.db.collection('ReadModel-StickerSetReadModel').insertOne(pack);
        importedPacks++;

        // Импортируем документы и файлы
        for (const doc of documents) {
          try {
            // Проверяем, существует ли документ
            const existingDoc = await req.db.collection('ReadModel-DocumentReadModel')
              .findOne({ DocumentId: doc.DocumentId });

            if (!existingDoc) {
              await req.db.collection('ReadModel-DocumentReadModel').insertOne(doc);
              importedDocuments++;
            }

            // Загружаем файл TGS в MinIO
            const documentId = doc.DocumentId.toString();
            const tgsEntry = zipEntries.find(e => e.entryName === `files/${documentId}.tgs`);

            if (tgsEntry) {
              const fileBuffer = tgsEntry.getData();

              // Проверяем, нет ли уже такого файла в MinIO
              try {
                await minioClient.statObject(MINIO_BUCKET, documentId);
                console.log(`File ${documentId} already exists in MinIO, skipping...`);
              } catch (err) {
                // Файла нет, загружаем его
                await minioClient.putObject(
                  MINIO_BUCKET,
                  documentId,
                  fileBuffer,
                  fileBuffer.length,
                  { 'Content-Type': 'application/x-tgsticker' }
                );
                importedFiles++;
              }
            } else {
              console.warn(`TGS file not found in ZIP for document ${documentId}`);
            }

          } catch (docError) {
            console.error(`Failed to import document ${doc.DocumentId}:`, docError.message);
          }
        }

        console.log(`Imported pack: ${pack.Title} (${documents.length} documents)`);

      } catch (packError) {
        console.error(`Failed to import pack:`, packError.message);
        errors.push({
          pack: packData.pack?.Title || 'Unknown',
          error: packError.message
        });
      }
    }

    console.log(`Import complete: ${importedPacks} packs, ${importedDocuments} documents, ${importedFiles} files imported`);

    res.json({
      success: true,
      imported: {
        packs: importedPacks,
        documents: importedDocuments,
        files: importedFiles
      },
      skipped: {
        packs: skippedPacks
      },
      errors: errors.length > 0 ? errors : undefined
    });

  } catch (error) {
    console.error('Import error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Получить один эмодзи-пак
router.get('/:id', async (req, res) => {
  try {
    const stickersetId = parseInt(req.params.id);

    const pack = await req.db.collection('ReadModel-StickerSetReadModel')
      .findOne({ StickerSetId: stickersetId });

    if (!pack) {
      return res.status(404).json({ error: 'Emoji pack not found' });
    }

    // Получаем все эмодзи этого пака
    const emojis = await req.db.collection('custom_emoji_documents')
      .find({ 'attributes.stickerset_id': stickersetId })
      .toArray();

    // Получаем группировку по эмодзи
    const emojiPacks = await req.db.collection('emoji_packs')
      .find({ stickerset_id: stickersetId })
      .toArray();

    res.json({
      pack,
      emojis,
      emoji_packs: emojiPacks
    });
  } catch (error) {
    console.error('Error fetching emoji pack:', error);
    res.status(500).json({ error: error.message });
  }
});

// Создать новый эмодзи-пак
router.post('/', async (req, res) => {
  try {
    const {
      title,
      short_name,
      emojis = true,
      text_color = false,
      channel_emoji_status = false,
      creator_id
    } = req.body;

    // Проверяем обязательные поля
    if (!title || !short_name) {
      return res.status(400).json({ error: 'Title and short_name are required' });
    }

    // Очищаем short_name: убираем пробелы и спецсимволы, оставляем только буквы и цифры
    const sanitized_short_name = short_name
      .replace(/[\s_\-()[\]{}!@#$%^&*+=<>?/\\|~`"':;,.]/g, '') // убираем пробелы и спецсимволы
      .replace(/[^a-zA-Z0-9]/g, ''); // оставляем только буквы и цифры

    if (!sanitized_short_name) {
      return res.status(400).json({ error: 'Short name must contain at least one alphanumeric character' });
    }

    // Проверяем, не занят ли short_name
    const existing = await req.db.collection('ReadModel-StickerSetReadModel')
      .findOne({ ShortName: sanitized_short_name });

    if (existing) {
      return res.status(409).json({ error: 'Short name already exists' });
    }

    // Генерируем stickerset_id
    const stickersetId = Date.now() * 10000 + Math.floor(Math.random() * 10000);
    const accessHash = Date.now() * 100000 + Math.floor(Math.random() * 100000);

    // Формат read-модели EventFlow
    const readModelId = `stickerset-${stickersetId}`;

    const pack = {
      _id: readModelId,  // EventFlow использует собственный _id
      Id: readModelId,
      Version: 1,
      StickerSetId: stickersetId,
      AccessHash: accessHash,
      ShortName: sanitized_short_name,
      Title: title,
      StickerSetType: 1, // CustomEmoji = 1 (Regular=0, CustomEmoji=1, Mask=2, System=3)
      Emojis: emojis,
      TextColor: text_color,
      ChannelEmojiStatus: channel_emoji_status,
      Masks: false,
      Count: 0,
      Packs: [],
      Keywords: [],
      StickerDocumentIds: [],
      Covers: [],
      Thumbs: null,
      ThumbVersion: null,
      ThumbDocumentId: null,
      CreatorId: creator_id ? parseInt(creator_id) : null,
      Archived: false,
      Official: false
    };

    // В EventFlow используется коллекция ReadModel-StickerSetReadModel
    await req.db.collection('ReadModel-StickerSetReadModel').insertOne(pack);

    res.status(201).json({
      message: 'Emoji pack created successfully',
      pack
    });
  } catch (error) {
    console.error('Error creating emoji pack:', error);
    res.status(500).json({ error: error.message });
  }
});

// Обновить эмодзи-пак
router.put('/:id', async (req, res) => {
  try {
    const stickersetId = parseInt(req.params.id);
    const { title, text_color, channel_emoji_status, archived } = req.body;

    const update = {
      $set: {
        ...(title && { Title: title }),
        ...(text_color !== undefined && { TextColor: text_color }),
        ...(channel_emoji_status !== undefined && { ChannelEmojiStatus: channel_emoji_status }),
        ...(archived !== undefined && { Archived: archived })
      },
      $inc: { Version: 1 }
    };

    const result = await req.db.collection('ReadModel-StickerSetReadModel')
      .updateOne({ StickerSetId: stickersetId }, update);

    if (result.matchedCount === 0) {
      return res.status(404).json({ error: 'Emoji pack not found' });
    }

    res.json({ message: 'Emoji pack updated successfully' });
  } catch (error) {
    console.error('Error updating emoji pack:', error);
    res.status(500).json({ error: error.message });
  }
});

// Удалить эмодзи-пак
router.delete('/:id', async (req, res) => {
  try {
    const stickersetId = parseInt(req.params.id);

    // Получаем набор стикеров, чтобы узнать ID документов
    const stickerSet = await req.db.collection('ReadModel-StickerSetReadModel')
      .findOne({ StickerSetId: stickersetId });

    if (!stickerSet) {
      return res.status(404).json({ error: 'Emoji pack not found' });
    }

    // Удаляем документы из ReadModel-DocumentReadModel (EventFlow)
    if (stickerSet.StickerDocumentIds && stickerSet.StickerDocumentIds.length > 0) {
      await req.db.collection('ReadModel-DocumentReadModel')
        .deleteMany({ DocumentId: { $in: stickerSet.StickerDocumentIds } });
    }

    // Удаляем все эмодзи из custom_emoji_documents (для обратной совместимости)
    await req.db.collection('custom_emoji_documents')
      .deleteMany({ 'attributes.stickerset_id': stickersetId });

    // Удаляем записи из emoji_packs (для обратной совместимости)
    await req.db.collection('emoji_packs')
      .deleteMany({ stickerset_id: stickersetId });

    // Удаляем из sticker_sets (для обратной совместимости)
    await req.db.collection('sticker_sets')
      .deleteOne({ stickerset_id: stickersetId });

    // Удаляем сам пак из ReadModel-StickerSetReadModel (EventFlow)
    const result = await req.db.collection('ReadModel-StickerSetReadModel')
      .deleteOne({ StickerSetId: stickersetId });

    res.json({ message: 'Emoji pack deleted successfully' });
  } catch (error) {
    console.error('Error deleting emoji pack:', error);
    res.status(500).json({ error: error.message });
  }
});

// Загрузить эмодзи TGS в пак
router.post('/:id/emojis', upload.single('file'), async (req, res) => {
  try {
    const stickersetId = parseInt(req.params.id);
    const { alt, is_free = 'true', has_text_color = 'false' } = req.body;

    if (!req.file) {
      return res.status(400).json({ error: 'No file uploaded' });
    }

    if (!alt) {
      return res.status(400).json({ error: 'Alt emoji is required' });
    }

    // Проверяем файл TGS и разбираем его как JSON
    const filePath = req.file.path;
    const fileBuffer = await fs.readFile(filePath);

    let isValid = true;
    let metadata = {};
    let jsonContent = null;
    let finalMimeType = req.file.mimetype;
    let finalBuffer = fileBuffer;

    try {
      if (finalMimeType === 'application/x-tgsticker') {
        // Пробуем распаковать и разобрать TGS
        const decompressed = await gunzip(fileBuffer);
        const json = JSON.parse(decompressed.toString('utf8'));

        // Проверяем структуру TGS
        if (json.tgs !== 1) isValid = false;
        if (json.w !== 512 || json.h !== 512) isValid = false;
        if (json.fr !== 60) isValid = false;

        metadata = {
          width: json.w,
          height: json.h,
          fps: json.fr,
          duration: (json.op - json.ip) / json.fr
        };

        if (metadata.duration > 3) {
          return res.status(400).json({ error: 'Animation duration exceeds 3 seconds' });
        }

        // Сохраняем формат TGS (gzip-сжатый JSON) — не распаковываем
        jsonContent = json;
        finalBuffer = fileBuffer; // оставляем исходный сжатый формат

        console.log(`TGS validated: ${finalBuffer.length} bytes (gzipped)`);
      } else if (finalMimeType === 'video/webm') {
        // Базовая проверка WebM
        metadata = { format: 'webm' };
      } else if (finalMimeType === 'image/webp') {
        // Проверка WebP
        metadata = { format: 'webp' };
      } else if (finalMimeType === 'image/png') {
        // Проверка PNG
        metadata = { format: 'png' };
      }
    } catch (err) {
      await fs.unlink(filePath);
      return res.status(400).json({ error: 'Invalid file format: ' + err.message });
    }

    if (!isValid) {
      await fs.unlink(filePath);
      return res.status(400).json({ error: 'Validation failed' });
    }

    // Генерируем document_id
    const documentId = Date.now() * 10000 + Math.floor(Math.random() * 10000);
    const accessHash = Date.now() * 100000 + Math.floor(Math.random() * 100000);

    // Сохраняем JSON во временный файл для загрузки в MinIO
    let uploadFilePath = filePath;
    if (finalMimeType === 'application/json') {
      uploadFilePath = filePath + '.json';
      await fs.writeFile(uploadFilePath, finalBuffer);
    }

    // Загружаем файл в MinIO
    try {
      await minioHelper.uploadFile(
        uploadFilePath,
        documentId,
        finalMimeType
      );
      console.log(`Uploaded document ${documentId} to MinIO as ${finalMimeType}`);
    } catch (error) {
      console.error('Failed to upload to MinIO:', error);
      await fs.unlink(filePath);
      if (uploadFilePath !== filePath) await fs.unlink(uploadFilePath).catch(() => { });
      return res.status(500).json({ error: 'Failed to upload file to storage' });
    }

    // Удаляем временные файлы
    await fs.unlink(filePath);
    if (uploadFilePath !== filePath) await fs.unlink(uploadFilePath).catch(() => { });

    // Формат read-модели документа в EventFlow
    const documentReadModelId = `document-${documentId}`;

    // Создаём атрибут TDocumentAttributeCustomEmoji
    const customEmojiAttribute = {
      _t: 'TDocumentAttributeCustomEmoji',
      Free: is_free === 'true' || is_free === true,
      TextColor: has_text_color === 'true' || has_text_color === true,
      Alt: alt,
      Stickerset: {
        _t: 'TInputStickerSetID',
        Id: stickersetId,
        AccessHash: 0 // заполнит сервер
      }
    };

    // Формируем FileReference (8 байт: 4 байта метки времени + 4 случайных байта)
    const fileRefBuffer = Buffer.allocUnsafe(8);
    fileRefBuffer.writeUInt32BE(Math.floor(Date.now() / 1000), 0);
    fileRefBuffer.writeUInt32BE(Math.floor(Math.random() * 0xFFFFFFFF), 4);

    const documentReadModel = {
      _id: documentReadModelId,
      Id: documentReadModelId,
      Version: 1,
      DocumentId: documentId,
      AccessHash: accessHash,
      FileReference: fileRefBuffer,
      DcId: 2,
      Date: Math.floor(Date.now() / 1000),
      MimeType: finalMimeType, // используем finalMimeType (для TGS это application/json)
      Size: finalBuffer.length, // размер finalBuffer (размер JSON)
      Name: `${documentId}`, // только documentId, без расширения и папки
      Md5CheckSum: null,
      CreatorId: null,
      Thumbs: null,
      ThumbId: null,
      VideoThumbs: null,
      VideoThumbId: null,
      Attributes: null,
      Attributes2: [customEmojiAttribute] // добавляем атрибут кастомного эмодзи
    };

    // Вставляем в коллекцию EventFlow
    await req.db.collection('ReadModel-DocumentReadModel').insertOne(documentReadModel);

    // Дублируем в custom_emoji_documents для обратной совместимости
    const emoji = {
      document_id: documentId,
      access_hash: accessHash,
      file_reference: fileRefBuffer,
      dc_id: 2,
      date: Math.floor(Date.now() / 1000),
      mime_type: finalMimeType,
      size: finalBuffer.length,
      file_path: req.file.path,
      md5_checksum: null,
      attributes: {
        type: 'custom_emoji',
        free: is_free === 'true' || is_free === true,
        text_color: has_text_color === 'true' || has_text_color === true,
        alt,
        stickerset_id: stickersetId,
        w: metadata.width || 512,
        h: metadata.height || 512,
        mask: false,
        mask_coords: null
      },
      creator_id: null,
      usage_count: 0,
      premium_only: !(is_free === 'true' || is_free === true),
      created_at: new Date()
    };

    await req.db.collection('custom_emoji_documents').insertOne(emoji);

    // Обновляем ReadModel-StickerSetReadModel (коллекция EventFlow).
    // Packs — это List<StickerPackItem>, где StickerPackItem = record(string Emoticon, List<long> Documents).
    // Нужно либо добавить документ в существующий пак, либо создать новый
    const existingSet = await req.db.collection('ReadModel-StickerSetReadModel')
      .findOne({ StickerSetId: stickersetId });

    if (!existingSet) {
      return res.status(404).json({ error: 'Sticker set not found' });
    }

    // Проверяем, есть ли уже пак с таким эмодзи
    const existingPackIndex = existingSet.Packs?.findIndex(p => p.Emoticon === alt) ?? -1;

    let updateOperation;
    if (existingPackIndex >= 0) {
      // Добавляем документ в существующий пак
      updateOperation = {
        $inc: { Count: 1, Version: 1 },
        $push: {
          StickerDocumentIds: documentId,
          [`Packs.${existingPackIndex}.Documents`]: documentId
        }
      };
    } else {
      // Создаём новый пак
      const newPack = {
        Emoticon: alt,
        Documents: [documentId]
      };
      updateOperation = {
        $inc: { Count: 1, Version: 1 },
        $push: {
          StickerDocumentIds: documentId,
          Packs: newPack
        }
      };
    }

    const updateResult = await req.db.collection('ReadModel-StickerSetReadModel')
      .updateOne({ StickerSetId: stickersetId }, updateOperation);

    if (updateResult.matchedCount === 0) {
      return res.status(404).json({ error: 'Sticker set not found' });
    }

    // Обновляем старые коллекции для обратной совместимости
    await req.db.collection('sticker_sets').updateOne(
      { stickerset_id: stickersetId },
      {
        $inc: { count: 1 },
        $push: { document_ids: documentId },
        $set: { updated_at: new Date() }
      }
    );

    await req.db.collection('emoji_packs').updateOne(
      { stickerset_id: stickersetId, emoticon: alt },
      {
        $addToSet: { document_ids: documentId },
        $setOnInsert: {
          stickerset_id: stickersetId,
          emoticon: alt
        }
      },
      { upsert: true }
    );

    res.status(201).json({
      message: 'Emoji uploaded successfully',
      emoji: {
        ...emoji,
        metadata
      }
    });
  } catch (error) {
    console.error('Error uploading emoji:', error);
    // При ошибке удаляем загруженный файл
    if (req.file) {
      await fs.unlink(req.file.path).catch(() => { });
    }
    res.status(500).json({ error: error.message });
  }
});

// Удалить эмодзи из пака
router.delete('/:packId/emojis/:emojiId', async (req, res) => {
  try {
    const stickersetId = parseInt(req.params.packId);
    const documentId = parseInt(req.params.emojiId);

    // Получаем сведения об эмодзи
    const emoji = await req.db.collection('custom_emoji_documents')
      .findOne({ document_id: documentId });

    if (!emoji) {
      return res.status(404).json({ error: 'Emoji not found' });
    }

    // Удаляем локальный файл
    try {
      await fs.unlink(emoji.file_path);
    } catch (err) {
      console.warn('Failed to delete local file:', err);
    }

    // Удаляем из MinIO
    try {
      await minioHelper.deleteFile(documentId);
    } catch (err) {
      console.warn('Failed to delete from MinIO:', err);
    }

    // Удаляем из ReadModel-DocumentReadModel (EventFlow)
    await req.db.collection('ReadModel-DocumentReadModel')
      .deleteOne({ DocumentId: documentId });

    // Удаляем из custom_emoji_documents (для обратной совместимости)
    await req.db.collection('custom_emoji_documents')
      .deleteOne({ document_id: documentId });

    // Обновляем ReadModel-StickerSetReadModel (EventFlow):
    // убираем документ из StickerDocumentIds и из массивов Packs.Documents
    await req.db.collection('ReadModel-StickerSetReadModel').updateOne(
      { StickerSetId: stickersetId },
      {
        $inc: { Count: -1, Version: 1 },
        $pull: {
          StickerDocumentIds: documentId,
          'Packs.$[].Documents': documentId
        }
      }
    );

    // Убираем опустевшие паки
    await req.db.collection('ReadModel-StickerSetReadModel').updateOne(
      { StickerSetId: stickersetId },
      {
        $pull: {
          Packs: { Documents: { $size: 0 } }
        }
      }
    );

    // Обновляем старые коллекции (для обратной совместимости)
    await req.db.collection('sticker_sets').updateOne(
      { stickerset_id: stickersetId },
      {
        $inc: { count: -1 },
        $pull: { document_ids: documentId },
        $set: { updated_at: new Date() }
      }
    );

    await req.db.collection('emoji_packs').updateOne(
      { stickerset_id: stickersetId, emoticon: emoji.attributes.alt },
      { $pull: { document_ids: documentId } }
    );

    res.json({ message: 'Emoji deleted successfully' });
  } catch (error) {
    console.error('Error deleting emoji:', error);
    res.status(500).json({ error: error.message });
  }
});

// Получить ссылку на пак
router.get('/:id/link', async (req, res) => {
  try {
    const stickersetId = parseInt(req.params.id);

    const pack = await req.db.collection('sticker_sets')
      .findOne({ stickerset_id: stickersetId });

    if (!pack) {
      return res.status(404).json({ error: 'Emoji pack not found' });
    }

    const link = `https://t.me/addemoji/${pack.short_name}`;

    res.json({
      short_name: pack.short_name,
      link,
      qr_code: `https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=${encodeURIComponent(link)}`
    });
  } catch (error) {
    console.error('Error generating pack link:', error);
    res.status(500).json({ error: error.message });
  }
});

// Получить статистику
router.get('/:id/stats', async (req, res) => {
  try {
    const stickersetId = parseInt(req.params.id);

    const [pack, totalUsage, premiumCount, freeCount] = await Promise.all([
      req.db.collection('sticker_sets').findOne({ stickerset_id: stickersetId }),
      req.db.collection('custom_emoji_documents').aggregate([
        { $match: { 'attributes.stickerset_id': stickersetId } },
        { $group: { _id: null, total: { $sum: '$usage_count' } } }
      ]).toArray(),
      req.db.collection('custom_emoji_documents').countDocuments({
        'attributes.stickerset_id': stickersetId,
        'attributes.free': false
      }),
      req.db.collection('custom_emoji_documents').countDocuments({
        'attributes.stickerset_id': stickersetId,
        'attributes.free': true
      })
    ]);

    res.json({
      pack_name: pack?.title || 'Unknown',
      total_emojis: pack?.count || 0,
      total_usage: totalUsage[0]?.total || 0,
      premium_emojis: premiumCount,
      free_emojis: freeCount,
      created_at: pack?.created_at
    });
  } catch (error) {
    console.error('Error fetching stats:', error);
    res.status(500).json({ error: error.message });
  }
});

// Массовая загрузка эмодзи из ZIP
const zipUpload = multer({
  storage: multer.diskStorage({
    destination: async (req, file, cb) => {
      const uploadDir = path.join(process.cwd(), 'uploads', 'temp');
      await fs.mkdir(uploadDir, { recursive: true });
      cb(null, uploadDir);
    },
    filename: (req, file, cb) => {
      cb(null, Date.now() + '-' + file.originalname);
    }
  }),
  limits: { fileSize: 100 * 1024 * 1024 }, // до 100 МБ
  fileFilter: (req, file, cb) => {
    if (file.originalname.endsWith('.zip')) {
      cb(null, true);
    } else {
      cb(new Error('Only ZIP files are allowed'));
    }
  }
});

router.post('/bulk-upload', zipUpload.single('zipFile'), async (req, res) => {
  try {
    if (!req.file) {
      return res.status(400).json({ error: 'No ZIP file uploaded' });
    }

    const { packId } = req.body;
    const zipPath = req.file.path;
    const extractDir = path.join(process.cwd(), 'uploads', 'temp', `extract-${Date.now()}`);

    // Динамически импортируем AdmZip
    const AdmZip = (await import('adm-zip')).default;

    try {
      // Распаковываем ZIP
      const zip = new AdmZip(zipPath);
      zip.extractAllTo(extractDir, true);

      // Ищем все файлы стикеров
      const findStickerFiles = async (dir) => {
        const files = [];
        const items = await fs.readdir(dir, { withFileTypes: true });

        for (const item of items) {
          const fullPath = path.join(dir, item.name);
          if (item.isDirectory()) {
            files.push(...await findStickerFiles(fullPath));
          } else if (item.name.match(/\.(tgs|webp|png|webm)$/i)) {
            files.push(fullPath);
          }
        }
        return files;
      };

      const stickerFiles = await findStickerFiles(extractDir);

      if (stickerFiles.length === 0) {
        return res.status(400).json({ error: 'No sticker files (TGS/WEBP/PNG/WEBM) found in ZIP' });
      }

      // Набор эмодзи для случайного выбора
      const emojis = [
        '😀', '😃', '😄', '😁', '😆', '😅', '🤣', '😂', '🙂', '🙃',
        '😉', '😊', '😇', '🥰', '😍', '🤩', '😘', '😗', '😚', '😙',
        '🥲', '😋', '😛', '😜', '🤪', '😝', '🤑', '🤗', '🤭', '🤫',
        '🤔', '🤐', '🤨', '😐', '😑', '😶', '😏', '😒', '🙄', '😬',
        '🤥', '😌', '😔', '😪', '🤤', '😴', '😷', '🤒', '🤕', '🤢',
        '🤮', '🤧', '🥵', '🥶', '🥴', '😵', '🤯', '🤠', '🥳', '😎',
        '🤓', '🧐', '😕', '😟', '🙁', '☹️', '😮', '😯', '😲', '😳',
        '🥺', '😦', '😧', '😨', '😰', '😥', '😢', '😭', '😱', '😖',
        '😣', '😞', '😓', '😩', '😫', '🥱', '😤', '😡', '😠', '🤬',
        '😈', '👿', '💀', '☠️', '💩', '🤡', '👹', '👺', '👻', '👽'
      ];

      const results = [];
      let successCount = 0;

      // Обрабатываем каждый файл
      for (const stickerFile of stickerFiles) {
        try {
          const filename = path.basename(stickerFile);
          const randomEmoji = emojis[Math.floor(Math.random() * emojis.length)];

          // Генерируем ID документа
          const documentId = Date.now() + Math.floor(Math.random() * 1000000);

          // Проверяем файл в зависимости от его типа
          let fileBuffer;
          let json = {};
          let mimeType;

          try {
            fileBuffer = await fs.readFile(stickerFile);
            const ext = path.extname(filename).toLowerCase();

            if (ext === '.tgs') {
              mimeType = 'application/x-tgsticker';
              const decompressed = await gunzip(fileBuffer);
              json = JSON.parse(decompressed.toString('utf8'));
            } else if (ext === '.webm') {
              mimeType = 'video/webm';
            } else if (ext === '.webp') {
              mimeType = 'image/webp';
            } else if (ext === '.png') {
              mimeType = 'image/png';
            }
          } catch (err) {
            console.error(`Failed to validate ${filename}:`, err.message);
            results.push({
              success: false,
              filename,
              error: `Invalid file: ${err.message}`
            });
            continue;
          }

          // Загружаем файл в MinIO
          const objectName = await minioHelper.uploadFile(stickerFile, documentId, mimeType);

          const accessHash = Date.now() * 100000 + Math.floor(Math.random() * 100000);

          // Создаём атрибут TDocumentAttributeCustomEmoji
          const customEmojiAttribute = {
            _t: 'TDocumentAttributeCustomEmoji',
            Free: false, // по умолчанию доступно только в premium
            TextColor: false,
            Alt: randomEmoji,
            Stickerset: {
              _t: 'TInputStickerSetID',
              Id: packId ? parseInt(packId) : 0,
              AccessHash: 0 // заполнит сервер
            }
          };

          // Формируем FileReference (8 байт)
          const fileRefBuffer = Buffer.allocUnsafe(8);
          fileRefBuffer.writeUInt32BE(Math.floor(Date.now() / 1000), 0);
          fileRefBuffer.writeUInt32BE(Math.floor(Math.random() * 0xFFFFFFFF), 4);

          // Создаём документ в ReadModel-DocumentReadModel (формат EventFlow)
          const documentReadModelId = `document-${documentId}`;
          const documentReadModel = {
            _id: documentReadModelId,
            Id: documentReadModelId,
            Version: 1,
            DocumentId: documentId,
            AccessHash: accessHash,
            FileReference: fileRefBuffer,
            DcId: 2,
            Date: Math.floor(Date.now() / 1000),
            MimeType: mimeType, // используем определённый MIME-тип
            Size: fileBuffer.length, // исходный размер (gzip)
            Name: `${documentId}`,
            Md5CheckSum: null,
            CreatorId: null,
            Thumbs: null,
            ThumbId: null,
            VideoThumbs: null,
            VideoThumbId: null,
            Attributes: null,
            Attributes2: [customEmojiAttribute] // добавляем атрибут кастомного эмодзи
          };

          await req.db.collection('ReadModel-DocumentReadModel').insertOne(documentReadModel);

          // Дублируем в custom_emoji_documents для обратной совместимости
          const document = {
            document_id: documentId,
            access_hash: accessHash,
            file_reference: fileRefBuffer,
            date: Math.floor(Date.now() / 1000),
            mime_type: mimeType,
            size: fileBuffer.length,
            dc_id: 2,
            attributes: {
              type: 'custom_emoji',
              stickerset_id: packId ? parseInt(packId) : null,
              alt: randomEmoji,
              w: json.w || 512,
              h: json.h || 512,
              mask: false,
              mask_coords: null,
              free: false,
              text_color: false
            },
            minio_object_name: objectName,
            premium_only: true,
            usage_count: 0,
            created_at: new Date()
          };

          await req.db.collection('custom_emoji_documents').insertOne(document);

          // Обновляем StickerSetReadModel: добавляем документ в нужную структуру пака
          if (packId) {
            const existingSet = await req.db.collection('ReadModel-StickerSetReadModel')
              .findOne({ StickerSetId: parseInt(packId) });

            if (existingSet) {
              // Проверяем, есть ли уже пак с таким эмодзи
              const existingPackIndex = existingSet.Packs?.findIndex(p => p.Emoticon === randomEmoji) ?? -1;

              let updateOperation;
              if (existingPackIndex >= 0) {
                // Добавляем документ в существующий пак
                updateOperation = {
                  $inc: { Count: 1, Version: 1 },
                  $push: {
                    StickerDocumentIds: documentId,
                    [`Packs.${existingPackIndex}.Documents`]: documentId
                  }
                };
              } else {
                // Создаём новый пак
                const newPack = {
                  Emoticon: randomEmoji,
                  Documents: [documentId]
                };
                updateOperation = {
                  $inc: { Count: 1, Version: 1 },
                  $push: {
                    StickerDocumentIds: documentId,
                    Packs: newPack
                  }
                };
              }

              await req.db.collection('ReadModel-StickerSetReadModel')
                .updateOne({ StickerSetId: parseInt(packId) }, updateOperation);
            }
          }

          results.push({
            success: true,
            filename,
            emoji: randomEmoji,
            documentId
          });
          successCount++;

        } catch (error) {
          console.error(`Error processing ${path.basename(stickerFile)}:`, error);
          results.push({
            success: false,
            filename: path.basename(stickerFile),
            error: error.message
          });
        }
      }

      // Очистка временных файлов
      await fs.rm(extractDir, { recursive: true, force: true });
      await fs.unlink(zipPath);

      res.json({
        success: true,
        totalFiles: stickerFiles.length,
        successCount,
        failedCount: stickerFiles.length - successCount,
        results,
        packId
      });

    } catch (error) {
      // Очистка при ошибке
      try {
        await fs.rm(extractDir, { recursive: true, force: true });
        await fs.unlink(zipPath);
      } catch (cleanupError) {
        console.error('Cleanup error:', cleanupError);
      }
      throw error;
    }

  } catch (error) {
    console.error('Bulk upload error:', error);
    res.status(500).json({ error: error.message });
  }
});

export default router;
