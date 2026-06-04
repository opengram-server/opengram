import express from 'express';
import multer from 'multer';
import path from 'path';
import fs from 'fs/promises';
import zlib from 'zlib';
import { promisify } from 'util';
import minioHelper from '../utils/minioHelper.js';

const gunzip = promisify(zlib.gunzip);
const router = express.Router();

// Настройка загрузки файлов стикеров (TGS, WebM, WebP, PNG)
const storage = multer.diskStorage({
  destination: async (req, file, cb) => {
    const uploadDir = path.join(process.cwd(), 'uploads', 'sticker-packs');
    await fs.mkdir(uploadDir, { recursive: true });
    cb(null, uploadDir);
  },
  filename: (req, file, cb) => {
    const uniqueSuffix = Date.now() + '-' + Math.round(Math.random() * 1E9);
    cb(null, 'sticker-' + uniqueSuffix + path.extname(file.originalname));
  }
});

const upload = multer({
  storage,
  limits: { fileSize: 512 * 1024 }, // до 512 КБ (стикеры крупнее эмодзи)
  fileFilter: (req, file, cb) => {
    const allowedTypes = ['.tgs', '.webm', '.webp', '.png'];
    const ext = path.extname(file.originalname).toLowerCase();

    if (allowedTypes.includes(ext)) {
      cb(null, true);
    } else {
      cb(new Error('Only TGS, WebM, WebP, and PNG files are allowed'));
    }
  }
});

// Список всех стикерпаков
router.get('/', async (req, res) => {
  try {
    const { page = 1, limit = 20, search = '' } = req.query;

    const filter = {
      Emojis: false // false для обычных стикеров (не кастомных эмодзи)
    };

    if (search) {
      filter.$or = [
        { Title: { $regex: search, $options: 'i' } },
        { ShortName: { $regex: search, $options: 'i' } }
      ];
    }

    const packs = await req.db.collection('ReadModel-StickerSetReadModel')
      .find(filter)
      .sort({ StickerSetId: -1 })
      .skip((page - 1) * limit)
      .limit(parseInt(limit))
      .toArray();

    const total = await req.db.collection('ReadModel-StickerSetReadModel')
      .countDocuments(filter);

    res.json({
      packs,
      pagination: {
        page: parseInt(page),
        limit: parseInt(limit),
        total,
        totalPages: Math.ceil(total / parseInt(limit))
      }
    });
  } catch (error) {
    console.error('Error fetching sticker packs:', error);
    res.status(500).json({ error: error.message });
  }
});

// Рекомендуемые стикерпаки (маршрут должен идти до /:id)
router.get('/featured', async (req, res) => {
  try {
    const packs = await req.db.collection('ReadModel-StickerSetReadModel').find({
      IsFeatured: true,
      Emojis: false  // только обычные стикеры, не кастомные эмодзи
    }).sort({ FeaturedOrder: 1 }).toArray();

    res.json({
      success: true,
      count: packs.length,
      packs
    });
  } catch (error) {
    console.error('Error fetching featured packs:', error);
    res.status(500).json({ error: error.message });
  }
});

// Изменить порядок рекомендуемых паков (маршрут должен идти до /:id)
router.post('/featured/reorder', async (req, res) => {
  try {
    const { packs } = req.body;

    for (const pack of packs) {
      await req.db.collection('ReadModel-StickerSetReadModel').updateOne(
        { StickerSetId: parseInt(pack.stickerSetId) },
        { $set: { FeaturedOrder: pack.featuredOrder } }
      );
    }

    res.json({ success: true });
  } catch (error) {
    console.error('Error reordering packs:', error);
    res.status(500).json({ error: error.message });
  }
});

// Получить один стикерпак
router.get('/:id', async (req, res) => {
  try {
    const stickersetId = parseInt(req.params.id);

    const pack = await req.db.collection('ReadModel-StickerSetReadModel')
      .findOne({ StickerSetId: stickersetId });

    if (!pack) {
      return res.status(404).json({ error: 'Sticker pack not found' });
    }

    // Получаем все стикеры (документы) этого пака
    const stickers = await req.db.collection('ReadModel-DocumentReadModel')
      .find({ DocumentId: { $in: pack.StickerDocumentIds || [] } })
      .toArray();

    res.json({
      pack,
      stickers
    });
  } catch (error) {
    console.error('Error fetching sticker pack:', error);
    res.status(500).json({ error: error.message });
  }
});

// Создать новый стикерпак
router.post('/', async (req, res) => {
  try {
    const {
      title,
      short_name,
      masks = false,
      creator_id
    } = req.body;

    // Проверяем обязательные поля
    if (!title || !short_name) {
      return res.status(400).json({ error: 'Title and short_name are required' });
    }

    // Проверяем формат short_name (только a-z, 0-9 и _)
    if (!/^[a-z0-9_]+$/.test(short_name)) {
      return res.status(400).json({
        error: 'Short name must contain only lowercase letters, numbers, and underscores'
      });
    }

    // Проверяем, не занят ли short_name
    const existing = await req.db.collection('ReadModel-StickerSetReadModel')
      .findOne({ ShortName: short_name });

    if (existing) {
      return res.status(409).json({ error: 'Short name already exists' });
    }

    // Генерируем stickerset_id
    const stickersetId = Date.now() * 10000 + Math.floor(Math.random() * 10000);
    const accessHash = Date.now() * 100000 + Math.floor(Math.random() * 100000);

    // Формат read-модели EventFlow
    const readModelId = `stickerset-${stickersetId}`;

    const pack = {
      _id: readModelId,
      Id: readModelId,
      Version: 1,
      StickerSetId: stickersetId,
      AccessHash: accessHash,
      ShortName: short_name,
      Title: title,
      StickerSetType: 0, // обычные стикеры = 0 (CustomEmoji = 2)
      Emojis: false, // false для стикеров, true для кастомных эмодзи
      TextColor: false,
      ChannelEmojiStatus: false,
      Masks: masks,
      Count: 0,
      Packs: [],
      Keywords: [],
      StickerDocumentIds: [],
      Covers: [],
      Thumbs: null,
      ThumbVersion: null,
      ThumbDocumentId: null
    };

    await req.db.collection('ReadModel-StickerSetReadModel').insertOne(pack);

    console.log(`Created sticker pack: ${title} (${short_name}), ID: ${stickersetId}`);

    res.status(201).json({
      message: 'Sticker pack created successfully',
      pack
    });
  } catch (error) {
    console.error('Error creating sticker pack:', error);
    res.status(500).json({ error: error.message });
  }
});

// Обновить стикерпак
router.put('/:id', async (req, res) => {
  try {
    const stickersetId = parseInt(req.params.id);
    const { title, masks, archived } = req.body;

    const update = {
      $set: {
        ...(title && { Title: title }),
        ...(masks !== undefined && { Masks: masks }),
        ...(archived !== undefined && { Archived: archived })
      },
      $inc: { Version: 1 }
    };

    const result = await req.db.collection('ReadModel-StickerSetReadModel')
      .updateOne({ StickerSetId: stickersetId }, update);

    if (result.matchedCount === 0) {
      return res.status(404).json({ error: 'Sticker pack not found' });
    }

    res.json({ message: 'Sticker pack updated successfully' });
  } catch (error) {
    console.error('Error updating sticker pack:', error);
    res.status(500).json({ error: error.message });
  }
});

// Удалить стикерпак
router.delete('/:id', async (req, res) => {
  try {
    const stickersetId = parseInt(req.params.id);

    // Получаем набор стикеров, чтобы узнать ID документов
    const stickerSet = await req.db.collection('ReadModel-StickerSetReadModel')
      .findOne({ StickerSetId: stickersetId });

    if (!stickerSet) {
      return res.status(404).json({ error: 'Sticker pack not found' });
    }

    // Удаляем документы из ReadModel-DocumentReadModel
    if (stickerSet.StickerDocumentIds && stickerSet.StickerDocumentIds.length > 0) {
      // Удаляем из MinIO
      for (const docId of stickerSet.StickerDocumentIds) {
        try {
          await minioHelper.deleteFile(docId);
        } catch (err) {
          console.warn(`Failed to delete document ${docId} from MinIO:`, err);
        }
      }

      // Удаляем из MongoDB
      await req.db.collection('ReadModel-DocumentReadModel')
        .deleteMany({ DocumentId: { $in: stickerSet.StickerDocumentIds } });
    }

    // Удаляем сам пак
    await req.db.collection('ReadModel-StickerSetReadModel')
      .deleteOne({ StickerSetId: stickersetId });

    console.log(`Deleted sticker pack: ${stickerSet.Title}, ID: ${stickersetId}`);

    res.json({ message: 'Sticker pack deleted successfully' });
  } catch (error) {
    console.error('Error deleting sticker pack:', error);
    res.status(500).json({ error: error.message });
  }
});

// Загрузить стикер в пак
router.post('/:id/stickers', upload.single('file'), async (req, res) => {
  try {
    const stickersetId = parseInt(req.params.id);
    const { emoji = '😀', creator_id } = req.body;

    console.log(`Starting sticker upload for pack ${stickersetId}`);
    console.log(`Initial DB check - Database:`, req.db?.databaseName || 'UNDEFINED');

    if (!req.file) {
      return res.status(400).json({ error: 'No file uploaded' });
    }

    if (!emoji) {
      return res.status(400).json({ error: 'Emoji is required' });
    }

    const filePath = req.file.path;
    const fileBuffer = await fs.readFile(filePath);
    const ext = path.extname(req.file.originalname).toLowerCase();

    let isValid = true;
    let metadata = {};
    let mimeType = req.file.mimetype;

    // Проверяем в зависимости от типа файла
    if (ext === '.tgs') {
      try {
        // Распаковываем и разбираем TGS
        const decompressed = await gunzip(fileBuffer);
        const json = JSON.parse(decompressed.toString('utf8'));

        // Проверяем структуру TGS для стикеров (512x512, а не 100x100 как у эмодзи)
        if (json.w !== 512 || json.h !== 512) {
          await fs.unlink(filePath);
          return res.status(400).json({
            error: `Invalid TGS dimensions: ${json.w}x${json.h}. Stickers must be 512x512 px`
          });
        }
        if (json.fr !== 60) {
          await fs.unlink(filePath);
          return res.status(400).json({ error: 'TGS framerate must be 60 FPS' });
        }

        const duration = (json.op - json.ip) / json.fr;
        if (duration > 3) {
          await fs.unlink(filePath);
          return res.status(400).json({ error: 'Animation duration exceeds 3 seconds' });
        }

        metadata = {
          width: json.w,
          height: json.h,
          fps: json.fr,
          duration
        };

        mimeType = 'application/x-tgsticker';
      } catch (err) {
        await fs.unlink(filePath);
        return res.status(400).json({ error: 'Invalid TGS file format: ' + err.message });
      }
    } else if (ext === '.webm') {
      // Базовая проверка WebM (должен быть 512x512, без звука, VP9)
      mimeType = 'video/webm';
      metadata = { format: 'webm' };
    } else if (ext === '.webp') {
      // Проверка WebP (должен быть 512x512)
      mimeType = 'image/webp';
      metadata = { format: 'webp' };
    } else if (ext === '.png') {
      // Проверка PNG (должен быть 512x512)
      mimeType = 'image/png';
      metadata = { format: 'png' };
    }

    // Генерируем document_id (тем же способом, что и в emojipacks)
    const documentId = Date.now() * 10000 + Math.floor(Math.random() * 10000);
    const accessHash = Date.now() * 100000 + Math.floor(Math.random() * 100000);

    // Сначала получаем набор стикеров (AccessHash нужен для Attributes2)
    const existingSet = await req.db.collection('ReadModel-StickerSetReadModel')
      .findOne({ StickerSetId: stickersetId });

    if (!existingSet) {
      await fs.unlink(filePath);
      return res.status(404).json({ error: 'Sticker set not found' });
    }

    // Загружаем файл в MinIO (без расширения)
    try {
      await minioHelper.uploadFile(filePath, documentId, mimeType);
      console.log(`Uploaded sticker document ${documentId} to MinIO (${mimeType})`);
    } catch (error) {
      console.error('Failed to upload to MinIO:', error);
      await fs.unlink(filePath);
      return res.status(500).json({ error: 'Failed to upload file to storage' });
    }

    // Формат read-модели документа в EventFlow
    const documentReadModelId = `document-${documentId}`;

    // Отладочный вывод сериализации буфера
    const fileRefBuffer = Buffer.from(`${documentId}`);
    console.log('FileReference buffer:', fileRefBuffer, 'toString:', fileRefBuffer.toString());

    const documentReadModel = {
      _id: documentReadModelId,
      Id: documentReadModelId,
      Version: 1,
      DocumentId: documentId,
      AccessHash: accessHash,
      FileReference: fileRefBuffer,
      DcId: 2,
      Date: Math.floor(Date.now() / 1000),
      MimeType: mimeType,
      Size: req.file.size,
      Name: `${documentId}`, // только documentId, без расширения
      Md5CheckSum: null,
      CreatorId: creator_id ? parseInt(creator_id) : null,
      Thumbs: null,
      ThumbId: null,
      VideoThumbs: null,
      VideoThumbId: null,
      Attributes: null,
      Attributes2: null // file-server из MyTelegram заполнит это автоматически
    };

    // Вставляем в коллекцию EventFlow
    console.log('Database name:', req.db.databaseName);
    console.log('Full documentReadModel:', JSON.stringify(documentReadModel, null, 2));
    console.log(`Inserting document into ReadModel-DocumentReadModel:`, {
      _id: documentReadModel._id,
      DocumentId: documentReadModel.DocumentId,
      MimeType: documentReadModel.MimeType
    });

    try {
      const insertResult = await req.db.collection('ReadModel-DocumentReadModel').insertOne(documentReadModel);
      console.log(`Document inserted successfully:`, insertResult.insertedId);
      console.log(`Acknowledged:`, insertResult.acknowledged);

      // Сразу проверяем, что документ записан
      const verifyDoc = await req.db.collection('ReadModel-DocumentReadModel').findOne({ DocumentId: documentId });
      console.log(`Verification after insert:`, verifyDoc ? 'FOUND' : 'NOT FOUND');
      if (verifyDoc) {
        console.log(`Verified _id:`, verifyDoc._id);
      }
    } catch (insertError) {
      console.error(`Failed to insert document:`, insertError);
      throw insertError;
    }

    // Обновляем ReadModel-StickerSetReadModel.
    // Проверяем, есть ли уже пак с таким эмодзи
    const existingPackIndex = existingSet.Packs?.findIndex(p => p.Emoticon === emoji) ?? -1;

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
        Emoticon: emoji,
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
      await fs.unlink(filePath);
      return res.status(404).json({ error: 'Sticker set not found' });
    }

    // Удаляем локальный файл
    await fs.unlink(filePath).catch(() => { });

    console.log(`Added sticker ${documentId} to pack ${stickersetId} with emoji ${emoji}`);

    res.status(201).json({
      message: 'Sticker uploaded successfully',
      sticker: {
        document_id: documentId,
        access_hash: accessHash,
        mime_type: mimeType,
        size: req.file.size,
        emoji,
        metadata
      }
    });
  } catch (error) {
    console.error('Error uploading sticker:', error);
    // При ошибке удаляем загруженный файл
    if (req.file) {
      await fs.unlink(req.file.path).catch(() => { });
    }
    res.status(500).json({ error: error.message });
  }
});

// Удалить стикер из пака
router.delete('/:packId/stickers/:stickerId', async (req, res) => {
  try {
    const stickersetId = parseInt(req.params.packId);
    const documentId = parseInt(req.params.stickerId);

    // Получаем сведения о стикере
    const sticker = await req.db.collection('ReadModel-DocumentReadModel')
      .findOne({ DocumentId: documentId });

    if (!sticker) {
      return res.status(404).json({ error: 'Sticker not found' });
    }

    // Удаляем из MinIO
    try {
      await minioHelper.deleteFile(documentId);
      console.log(`Deleted sticker ${documentId} from MinIO`);
    } catch (err) {
      console.warn('Failed to delete from MinIO:', err);
    }

    // Удаляем из ReadModel-DocumentReadModel
    await req.db.collection('ReadModel-DocumentReadModel')
      .deleteOne({ DocumentId: documentId });

    // Обновляем ReadModel-StickerSetReadModel
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

    console.log(`Deleted sticker ${documentId} from pack ${stickersetId}`);

    res.json({ message: 'Sticker deleted successfully' });
  } catch (error) {
    console.error('Error deleting sticker:', error);
    res.status(500).json({ error: error.message });
  }
});

// Получить ссылку на пак
router.get('/:id/link', async (req, res) => {
  try {
    const stickersetId = parseInt(req.params.id);

    const pack = await req.db.collection('ReadModel-StickerSetReadModel')
      .findOne({ StickerSetId: stickersetId });

    if (!pack) {
      return res.status(404).json({ error: 'Sticker pack not found' });
    }

    const link = `https://t.me/addstickers/${pack.ShortName}`;

    res.json({
      short_name: pack.ShortName,
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

    const pack = await req.db.collection('ReadModel-StickerSetReadModel')
      .findOne({ StickerSetId: stickersetId });

    if (!pack) {
      return res.status(404).json({ error: 'Sticker pack not found' });
    }

    // Считаем по MIME-типам
    const stickers = await req.db.collection('ReadModel-DocumentReadModel')
      .find({ DocumentId: { $in: pack.StickerDocumentIds || [] } })
      .toArray();

    const stats = {
      tgs: stickers.filter(s => s.MimeType === 'application/x-tgsticker').length,
      webm: stickers.filter(s => s.MimeType === 'video/webm').length,
      webp: stickers.filter(s => s.MimeType === 'image/webp').length,
      png: stickers.filter(s => s.MimeType === 'image/png').length
    };

    res.json({
      pack_name: pack.Title,
      short_name: pack.ShortName,
      total_stickers: pack.Count,
      sticker_types: stats,
      is_masks: pack.Masks,
      created_at: pack.CreatedAt
    });
  } catch (error) {
    console.error('Error fetching stats:', error);
    res.status(500).json({ error: error.message });
  }
});

// Настройка загрузки ZIP
const zipUpload = multer({
  storage: multer.diskStorage({
    destination: async (req, file, cb) => {
      const uploadDir = path.join(process.cwd(), 'uploads', 'zip');
      await fs.mkdir(uploadDir, { recursive: true });
      cb(null, uploadDir);
    },
    filename: (req, file, cb) => {
      cb(null, `stickers-${Date.now()}.zip`);
    }
  }),
  limits: { fileSize: 500 * 1024 * 1024 } // до 500 МБ
});

// Массовая загрузка стикеров из ZIP
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

      // Ищем все файлы стикеров (TGS, WEBP, PNG)
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
        return res.status(400).json({ error: 'No sticker files (TGS/WEBP/PNG) found in ZIP' });
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

      // Premium-стикеры: группируем файлы по префиксу (000_main.tgs + 000_effect.tgs)
      const premiumGroups = new Map(); // Map<prefix, {main, effect}>
      const regularFiles = [];
      const newDocuments = []; // собираем все новые документы, чтобы обновить пак в конце

      for (const file of stickerFiles) {
        const filename = path.basename(file);

        // Проверяем premium-формат: XXX_main.tgs или XXX_effect.tgs
        const premiumMatch = filename.match(/^(\d+)_(main|effect)\.(tgs|webp|png)$/i);

        if (premiumMatch) {
          const [, prefix, type] = premiumMatch;

          if (!premiumGroups.has(prefix)) {
            premiumGroups.set(prefix, { main: null, effect: null });
          }

          const group = premiumGroups.get(prefix);
          if (type.toLowerCase() === 'main') {
            group.main = file;
          } else if (type.toLowerCase() === 'effect') {
            group.effect = file;
          }
        } else {
          // Обычный стикер (без суффикса _main/_effect)
          regularFiles.push(file);
        }
      }

      const results = [];
      let successCount = 0;
      let premiumCount = 0;

      // Обрабатываем premium-стикеры (с эффектами)
      for (const [prefix, group] of premiumGroups) {
        if (!group.main) {
          console.log(`Skipping ${prefix}: no main file`);
          continue;
        }

        console.log(`\nProcessing sticker ${prefix}:`);
        console.log(`  Main: ${group.main ? 'yes' : 'no'}`);
        console.log(`  Effect: ${group.effect ? 'yes' : 'no'}`);

        try {
          const filename = path.basename(group.main);
          const randomEmoji = emojis[Math.floor(Math.random() * emojis.length)];

          // Генерируем ID документа для основного стикера
          const documentId = Date.now() + Math.floor(Math.random() * 1000000);

          // Читаем основной файл
          const mainBuffer = await fs.readFile(group.main);
          const ext = path.extname(filename).toLowerCase();
          let mimeType = ext === '.tgs' ? 'application/x-tgsticker' :
            ext === '.webm' ? 'video/webm' :
              ext === '.webp' ? 'image/webp' : 'image/png';

          // Проверяем TGS
          if (ext === '.tgs') {
            try {
              const decompressed = await gunzip(mainBuffer);
              JSON.parse(decompressed.toString('utf8'));
            } catch (err) {
              throw new Error(`Invalid TGS file: ${err.message}`);
            }
          }

          // Загружаем основной стикер в MinIO
          const mainObjectName = await minioHelper.uploadFile(group.main, documentId, mimeType);
          const accessHash = Date.now() * 100000 + Math.floor(Math.random() * 100000);

          // Premium: загружаем эффект, если он есть
          let videoThumbs = null;
          let effectDocumentId = null;

          if (group.effect) {
            try {
              effectDocumentId = Date.now() + Math.floor(Math.random() * 1000000);
              const effectBuffer = await fs.readFile(group.effect);

              // Проверяем TGS эффекта
              const decompressed = await gunzip(effectBuffer);
              JSON.parse(decompressed.toString('utf8'));

              // Загружаем эффект в MinIO под отдельным ID документа
              await minioHelper.uploadFile(group.effect, effectDocumentId, 'application/x-tgsticker');

              // Формируем массив VideoThumbs для premium-стикера
              videoThumbs = [
                {
                  Type: 'f',  // type="f" — premium-эффект
                  W: 512,
                  H: 512,
                  Size: effectBuffer.length
                }
              ];

              console.log(`Premium effect uploaded: ${effectDocumentId}`);
            } catch (err) {
              console.error(`Failed to upload effect for ${prefix}: ${err.message}`);
            }
          }

          // Формируем FileReference (8 байт)
          const fileRefBuffer = Buffer.allocUnsafe(8);
          fileRefBuffer.writeUInt32BE(Math.floor(Date.now() / 1000), 0);
          fileRefBuffer.writeUInt32BE(Math.floor(Math.random() * 0xFFFFFFFF), 4);

          // Создаём атрибут стикера
          const stickerAttribute = {
            _t: 'TDocumentAttributeSticker',
            Mask: false,
            Alt: randomEmoji,
            Stickerset: {
              _t: 'TInputStickerSetID',
              Id: packId ? parseInt(packId) : 0,
              AccessHash: 0
            }
          };

          // Создаём документ в MongoDB
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
            MimeType: mimeType,
            Size: mainBuffer.length,
            Name: `${documentId}`,
            Md5CheckSum: null,
            CreatorId: null,
            Thumbs: null,
            ThumbId: null,
            VideoThumbs: videoThumbs,  // метаданные premium-эффекта
            VideoThumbId: effectDocumentId,  // ID документа эффекта
            Attributes: null,
            Attributes2: [stickerAttribute]
          };

          await req.db.collection('ReadModel-DocumentReadModel').insertOne(documentReadModel);

          // Запоминаем для массового обновления
          if (packId) {
            newDocuments.push({
              id: documentId,
              emoji: randomEmoji
            });
          }

          results.push({
            success: true,
            filename,
            documentId,
            emoji: randomEmoji,
            premium: !!group.effect,
            effectDocumentId
          });

          successCount++;
          if (group.effect) premiumCount++;

        } catch (err) {
          console.error(`Failed to process premium sticker ${prefix}:`, err);
          results.push({
            success: false,
            filename: path.basename(group.main),
            error: err.message
          });
        }
      }

      // Обрабатываем обычные стикеры (без эффектов)
      for (const stickerFile of regularFiles) {
        try {
          const filename = path.basename(stickerFile);
          const randomEmoji = emojis[Math.floor(Math.random() * emojis.length)];

          // Генерируем ID документа
          const documentId = Date.now() + Math.floor(Math.random() * 1000000);

          // Читаем файл
          const fileBuffer = await fs.readFile(stickerFile);
          const ext = path.extname(filename).toLowerCase();
          let mimeType;

          // Определяем MIME-тип
          if (ext === '.tgs') {
            mimeType = 'application/x-tgsticker';
            // Проверяем файл TGS (должен быть gzip-сжатым JSON)
            try {
              const decompressed = await gunzip(fileBuffer);
              JSON.parse(decompressed.toString('utf8'));
            } catch (err) {
              throw new Error(`Invalid TGS file: ${err.message}`);
            }
          } else if (ext === '.webm') {
            mimeType = 'video/webm';
          } else if (ext === '.webp') {
            mimeType = 'image/webp';
          } else {
            mimeType = 'image/png';
          }

          // Загружаем в MinIO
          const objectName = await minioHelper.uploadFile(stickerFile, documentId, mimeType);

          const accessHash = Date.now() * 100000 + Math.floor(Math.random() * 100000);

          // Создаём атрибут TDocumentAttributeSticker
          const stickerAttribute = {
            _t: 'TDocumentAttributeSticker',
            Mask: false,
            Alt: randomEmoji,
            Stickerset: {
              _t: 'TInputStickerSetID',
              Id: packId ? parseInt(packId) : 0,
              AccessHash: 0
            }
          };

          // Формируем FileReference (8 байт)
          const fileRefBuffer = Buffer.allocUnsafe(8);
          fileRefBuffer.writeUInt32BE(Math.floor(Date.now() / 1000), 0);
          fileRefBuffer.writeUInt32BE(Math.floor(Math.random() * 0xFFFFFFFF), 4);

          // Создаём документ в ReadModel-DocumentReadModel
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
            MimeType: mimeType,
            Size: fileBuffer.length,
            Name: `${documentId}`,
            Md5CheckSum: null,
            CreatorId: null,
            Thumbs: null,
            ThumbId: null,
            VideoThumbs: null,
            VideoThumbId: null,
            Attributes: null,
            Attributes2: [stickerAttribute]
          };

          await req.db.collection('ReadModel-DocumentReadModel').insertOne(documentReadModel);

          // Запоминаем для массового обновления
          if (packId) {
            newDocuments.push({
              id: documentId,
              emoji: randomEmoji
            });
          }

          results.push({
            success: true,
            filename,
            documentId,
            emoji: randomEmoji
          });
          successCount++;

        } catch (err) {
          console.error(`Failed to process ${path.basename(stickerFile)}:`, err);
          results.push({
            success: false,
            filename: path.basename(stickerFile),
            error: err.message
          });
        }
      }

      // Массовое обновление набора стикеров (Packs и счётчики)
      if (packId && newDocuments.length > 0) {
        try {
          const stickerSetId = parseInt(packId);
          const stickerSet = await req.db.collection('ReadModel-StickerSetReadModel').findOne({ StickerSetId: stickerSetId });

          if (stickerSet) {
            const packs = stickerSet.Packs || [];

            // Добавляем новые документы в Packs
            for (const doc of newDocuments) {
              const existingPack = packs.find(p => p.Emoticon === doc.emoji);
              if (existingPack) {
                existingPack.Documents.push(doc.id);
              } else {
                packs.push({
                  Emoticon: doc.emoji,
                  Documents: [doc.id]
                });
              }
            }

            // Обновляем БД
            await req.db.collection('ReadModel-StickerSetReadModel').updateOne(
              { StickerSetId: stickerSetId },
              {
                $set: { Packs: packs },
                $inc: { Count: newDocuments.length },
                $push: { StickerDocumentIds: { $each: newDocuments.map(d => d.id) } }
              }
            );
            console.log(`Updated sticker set ${stickerSetId}: added ${newDocuments.length} stickers`);
          } else {
            console.error(`Sticker set ${stickerSetId} not found for update`);
          }
        } catch (updateErr) {
          console.error('Failed to update sticker set:', updateErr);
        }
      }

      // Очистка временных файлов
      await fs.rm(extractDir, { recursive: true, force: true });
      await fs.unlink(zipPath);

      res.json({
        success: true,
        packId: packId ? parseInt(packId) : null,
        totalFiles: stickerFiles.length,
        successCount,
        premiumCount,  // число premium-стикеров с эффектами
        regularCount: successCount - premiumCount,
        results
      });

    } catch (err) {
      console.error('Error processing ZIP:', err);
      await fs.rm(extractDir, { recursive: true, force: true }).catch(() => { });
      await fs.unlink(zipPath).catch(() => { });
      res.status(500).json({ error: err.message });
    }
  } catch (error) {
    console.error('Error in bulk upload:', error);
    res.status(500).json({ error: error.message });
  }
});

// Массовая загрузка стикерпаков из вложенного ZIP (основной ZIP содержит ZIP-архивы паков)
router.post('/bulk-upload-nested', zipUpload.single('zipFile'), async (req, res) => {
  try {
    if (!req.file) {
      return res.status(400).json({ error: 'No ZIP file uploaded' });
    }

    const zipPath = req.file.path;
    const mainExtractDir = path.join(process.cwd(), 'uploads', 'temp', `nested-${Date.now()}`);

    // Импортируем AdmZip
    const AdmZip = (await import('adm-zip')).default;

    try {
      console.log(`\n${'='.repeat(60)}`);
      console.log('NESTED ZIP BULK UPLOAD STARTED');
      console.log(`${'='.repeat(60)}\n`);

      // Распаковываем основной ZIP
      const mainZip = new AdmZip(zipPath);
      mainZip.extractAllTo(mainExtractDir, true);
      console.log(`Extracted main ZIP to: ${mainExtractDir}`);

      // Ищем все файлы .zip (это архивы паков)
      const findPackZips = async (dir) => {
        const zips = [];
        const items = await fs.readdir(dir, { withFileTypes: true });

        for (const item of items) {
          const fullPath = path.join(dir, item.name);
          if (item.isDirectory()) {
            zips.push(...await findPackZips(fullPath));
          } else if (item.name.endsWith('.zip')) {
            zips.push(fullPath);
          }
        }
        return zips;
      };

      const packZips = await findPackZips(mainExtractDir);

      if (packZips.length === 0) {
        await fs.rm(mainExtractDir, { recursive: true, force: true });
        await fs.unlink(zipPath);
        return res.status(400).json({ error: 'No pack ZIP files found in main ZIP' });
      }

      console.log(`Found ${packZips.length} pack ZIPs\n`);

      const createdPacks = [];
      let totalStickersUploaded = 0;
      let failedPacks = [];

      // Обрабатываем каждый архив пака
      for (let i = 0; i < packZips.length; i++) {
        const packZipPath = packZips[i];
        const packZipName = path.basename(packZipPath, '.zip');

        console.log(`\n[${i + 1}/${packZips.length}] Processing: ${packZipName}`);
        console.log(`${'─'.repeat(40)}`);

        try {
          // Формируем название пака из имени файла (подчёркивания меняем на пробелы)
          const packTitle = packZipName.replace(/_/g, ' ');
          const shortName = packZipName.replace(/_/g, ''); // убираем подчёркивания: "Bloody_Font_Emoji" -> "BloodyFontEmoji"

          console.log(`  Title: ${packTitle}`);
          console.log(`  Short name: ${shortName}`);

          // Проверяем, не занят ли short_name
          const existing = await req.db.collection('ReadModel-StickerSetReadModel')
            .findOne({ ShortName: shortName });

          if (existing) {
            console.log(`  Pack with short name ${shortName} already exists, skipping`);
            failedPacks.push({ pack: packTitle, reason: 'Short name already exists' });
            continue;
          }

          // Генерируем stickerset_id
          const stickersetId = Date.now() * 10000 + Math.floor(Math.random() * 10000);
          const accessHash = Date.now() * 100000 + Math.floor(Math.random() * 100000);

          // Создаём пак в БД
          const readModelId = `stickerset-${stickersetId}`;
          const pack = {
            _id: readModelId,
            Id: readModelId,
            Version: 1,
            StickerSetId: stickersetId,
            AccessHash: accessHash,
            ShortName: shortName,
            Title: packTitle,
            StickerSetType: 1, // 1 = CustomEmoji (0=Regular, 1=CustomEmoji, 2=Mask)
            Emojis: true, // true для паков кастомных эмодзи
            IsFeatured: true, // автоматически добавляем в рекомендуемые
            FeaturedOrder: i + 1, // задаём порядок
            TextColor: false,
            ChannelEmojiStatus: false,
            Masks: false,
            Count: 0,
            Packs: [],
            Keywords: [],
            StickerDocumentIds: [],
            Covers: [],
            Thumbs: null,
            ThumbVersion: null,
            ThumbDocumentId: null
          };

          await req.db.collection('ReadModel-StickerSetReadModel').insertOne(pack);
          console.log(`  Created sticker pack ID: ${stickersetId}`);

          // Распаковываем архив пака
          const packExtractDir = path.join(mainExtractDir, `pack-${stickersetId}`);
          const packZip = new AdmZip(packZipPath);
          packZip.extractAllTo(packExtractDir, true);

          // Ищем все файлы .tgs
          const findTgsFiles = async (dir) => {
            const files = [];
            const items = await fs.readdir(dir, { withFileTypes: true });

            for (const item of items) {
              const fullPath = path.join(dir, item.name);
              if (item.isDirectory()) {
                files.push(...await findTgsFiles(fullPath));
              } else if (item.name.endsWith('.tgs')) {
                files.push(fullPath);
              }
            }
            return files;
          };

          const tgsFiles = await findTgsFiles(packExtractDir);
          console.log(`  Found ${tgsFiles.length} .tgs files`);

          if (tgsFiles.length === 0) {
            console.log(`  No .tgs files found, deleting pack`);
            await req.db.collection('ReadModel-StickerSetReadModel')
              .deleteOne({ StickerSetId: stickersetId });
            failedPacks.push({ pack: packTitle, reason: 'No .tgs files found' });
            continue;
          }

          // Загружаем все стикеры параллельно (пачками по 50)
          const packEmojis = new Map(); // emoji -> [documentIds]
          let uploadedCount = 0;
          const BATCH_SIZE = 50;

          console.log(`  Uploading in batches of ${BATCH_SIZE}...`);

          for (let i = 0; i < tgsFiles.length; i += BATCH_SIZE) {
            const batch = tgsFiles.slice(i, i + BATCH_SIZE);

            const uploadPromises = batch.map(async (tgsFile) => {
              try {
                const filename = path.basename(tgsFile);

                // Достаём эмодзи из имени файла: "🦆_12345.tgs" -> emoji="🦆"
                let emoji = '😀'; // значение по умолчанию
                const emojiMatch = filename.match(/^(.+?)_\d+\.tgs$/);
                if (emojiMatch) {
                  emoji = emojiMatch[1];
                }

                // Читаем и проверяем TGS
                const fileBuffer = await fs.readFile(tgsFile);

                try {
                  const decompressed = await gunzip(fileBuffer);
                  JSON.parse(decompressed.toString('utf8'));
                } catch (err) {
                  return { success: false, filename };
                }

                // Генерируем ID документа
                const documentId = Date.now() * 10000 + Math.floor(Math.random() * 10000);
                const docAccessHash = Date.now() * 100000 + Math.floor(Math.random() * 100000);

                // Загружаем в MinIO
                await minioHelper.uploadFile(tgsFile, documentId, 'application/x-tgsticker');

                // Создаём DocumentReadModel
                const documentReadModelId = `document-${documentId}`;
                const fileRefBuffer = Buffer.from(`${documentId}`);

                const documentReadModel = {
                  _id: documentReadModelId,
                  Id: documentReadModelId,
                  Version: 1,
                  DocumentId: documentId,
                  AccessHash: docAccessHash,
                  FileReference: fileRefBuffer,
                  DcId: 2,
                  Date: Math.floor(Date.now() / 1000),
                  MimeType: 'application/x-tgsticker',
                  Size: fileBuffer.length,
                  Name: `${documentId}`,
                  Md5CheckSum: null,
                  CreatorId: null,
                  Thumbs: null,
                  ThumbId: null,
                  VideoThumbs: null,
                  VideoThumbId: null,
                  Attributes: null,
                  Attributes2: null
                };

                await req.db.collection('ReadModel-DocumentReadModel').insertOne(documentReadModel);

                return { success: true, emoji, documentId };

              } catch (fileErr) {
                return { success: false, filename: path.basename(tgsFile), error: fileErr.message };
              }
            });

            // Wait for batch to complete
            const results = await Promise.all(uploadPromises);

            // Process results
            results.forEach(result => {
              if (result.success) {
                if (!packEmojis.has(result.emoji)) {
                  packEmojis.set(result.emoji, []);
                }
                packEmojis.get(result.emoji).push(result.documentId);
                uploadedCount++;
              }
            });

            console.log(`    Progress: ${uploadedCount}/${tgsFiles.length}`);
          }

          console.log(`  Uploaded ${uploadedCount}/${tgsFiles.length} stickers`);

          // Формируем массив Packs
          const packsArray = Array.from(packEmojis.entries()).map(([emoticon, documents]) => ({
            Emoticon: emoticon,
            Documents: documents
          }));

          // Собираем все ID документов
          const allDocumentIds = Array.from(packEmojis.values()).flat();

          // Обновляем пак всеми стикерами
          await req.db.collection('ReadModel-StickerSetReadModel').updateOne(
            { StickerSetId: stickersetId },
            {
              $set: {
                Count: uploadedCount,
                Packs: packsArray,
                StickerDocumentIds: allDocumentIds
              },
              $inc: { Version: 1 }
            }
          );

          createdPacks.push({
            title: packTitle,
            shortName,
            stickersetId,
            stickersCount: uploadedCount
          });

          totalStickersUploaded += uploadedCount;

        } catch (packErr) {
          console.error(`  Failed to process pack ${packZipName}:`, packErr);
          failedPacks.push({ pack: packZipName, reason: packErr.message });
        }
      }

      // Очистка временных файлов
      await fs.rm(mainExtractDir, { recursive: true, force: true });
      await fs.unlink(zipPath);

      console.log(`\n${'='.repeat(60)}`);
      console.log('NESTED ZIP BULK UPLOAD COMPLETED');
      console.log(`${'='.repeat(60)}`);
      console.log(`Total packs created: ${createdPacks.length}`);
      console.log(`Total stickers uploaded: ${totalStickersUploaded}`);
      console.log(`Failed packs: ${failedPacks.length}\n`);

      res.json({
        success: true,
        totalPacks: packZips.length,
        createdPacks: createdPacks.length,
        failedPacks: failedPacks.length,
        totalStickers: totalStickersUploaded,
        packs: createdPacks,
        failures: failedPacks
      });

    } catch (err) {
      console.error('Error processing nested ZIP:', err);
      await fs.rm(mainExtractDir, { recursive: true, force: true }).catch(() => { });
      await fs.unlink(zipPath).catch(() => { });
      res.status(500).json({ error: err.message });
    }
  } catch (error) {
    console.error('Error in nested bulk upload:', error);
    res.status(500).json({ error: error.message });
  }
});


// Массовая загрузка обычных стикерпаков из вложенного ZIP (основной ZIP содержит ZIP-архивы паков)
router.post('/bulk-upload-stickers', zipUpload.single('zipFile'), async (req, res) => {
  try {
    if (!req.file) {
      return res.status(400).json({ error: 'No ZIP file uploaded' });
    }

    const zipPath = req.file.path;
    const mainExtractDir = path.join(process.cwd(), 'uploads', 'temp', `stickers-${Date.now()}`);

    // Импортируем AdmZip
    const AdmZip = (await import('adm-zip')).default;

    try {
      console.log(`\n${'='.repeat(60)}`);
      console.log('REGULAR STICKER PACKS BULK UPLOAD STARTED');
      console.log(`${'='.repeat(60)}\n`);

      // Распаковываем основной ZIP
      const mainZip = new AdmZip(zipPath);
      mainZip.extractAllTo(mainExtractDir, true);
      console.log(`Extracted main ZIP to: ${mainExtractDir}`);

      // Ищем все файлы .zip (это архивы паков)
      const findPackZips = async (dir) => {
        const zips = [];
        const items = await fs.readdir(dir, { withFileTypes: true });

        for (const item of items) {
          const fullPath = path.join(dir, item.name);
          if (item.isDirectory()) {
            zips.push(...await findPackZips(fullPath));
          } else if (item.name.endsWith('.zip')) {
            zips.push(fullPath);
          }
        }
        return zips;
      };

      const packZips = await findPackZips(mainExtractDir);

      if (packZips.length === 0) {
        await fs.rm(mainExtractDir, { recursive: true, force: true });
        await fs.unlink(zipPath);
        return res.status(400).json({ error: 'No pack ZIP files found in main ZIP' });
      }

      console.log(`Found ${packZips.length} pack ZIPs\n`);

      const createdPacks = [];
      let totalStickersUploaded = 0;
      let failedPacks = [];

      // Обрабатываем каждый архив пака
      for (let i = 0; i < packZips.length; i++) {
        const packZipPath = packZips[i];
        const packZipName = path.basename(packZipPath, '.zip');

        console.log(`\n[${i + 1}/${packZips.length}] Processing: ${packZipName}`);
        console.log(`${'─'.repeat(40)}`);

        try {
          // Формируем название пака из имени файла (подчёркивания меняем на пробелы)
          const packTitle = packZipName.replace(/_/g, ' ');

          // Формируем короткое имя: убираем все спецсимволы, оставляем только буквы и цифры.
          // "Boy's_Club" -> "BoysClub", "Rick_and_Morty" -> "RickandMorty"
          const shortName = packZipName.replace(/[^a-zA-Z0-9]/g, '');

          console.log(`  Title: ${packTitle}`);
          console.log(`  Short name: ${shortName}`);

          // Проверяем, не занят ли short_name
          const existing = await req.db.collection('ReadModel-StickerSetReadModel')
            .findOne({ ShortName: shortName });

          if (existing) {
            console.log(`  Pack with short name ${shortName} already exists, skipping`);
            failedPacks.push({ pack: packTitle, reason: 'Short name already exists' });
            continue;
          }

          // Generate stickerset_id
          const stickersetId = Date.now() * 10000 + Math.floor(Math.random() * 10000);
          const accessHash = Date.now() * 100000 + Math.floor(Math.random() * 100000);

          // Create pack in database
          const readModelId = `stickerset-${stickersetId}`;
          const pack = {
            _id: readModelId,
            Id: readModelId,
            Version: 1,
            StickerSetId: stickersetId,
            AccessHash: accessHash,
            ShortName: shortName,
            Title: packTitle,
            StickerSetType: 0, // 0 = обычные стикеры (не CustomEmoji)
            Emojis: false, // false для обычных стикеров
            IsFeatured: true, // автоматически добавляем в рекомендуемые
            FeaturedOrder: i + 1, // задаём порядок
            TextColor: false,
            ChannelEmojiStatus: false,
            Masks: false,
            Count: 0,
            Packs: [],
            Keywords: [],
            StickerDocumentIds: [],
            Covers: [],
            Thumbs: null,
            ThumbVersion: null,
            ThumbDocumentId: null
          };

          await req.db.collection('ReadModel-StickerSetReadModel').insertOne(pack);
          console.log(`  Created sticker pack ID: ${stickersetId}`);

          // Распаковываем архив пака
          const packExtractDir = path.join(mainExtractDir, `pack-${stickersetId}`);
          const packZip = new AdmZip(packZipPath);
          packZip.extractAllTo(packExtractDir, true);

          // Ищем все файлы .tgs
          const findTgsFiles = async (dir) => {
            const files = [];
            const items = await fs.readdir(dir, { withFileTypes: true });

            for (const item of items) {
              const fullPath = path.join(dir, item.name);
              if (item.isDirectory()) {
                files.push(...await findTgsFiles(fullPath));
              } else if (item.name.endsWith('.tgs')) {
                files.push(fullPath);
              }
            }
            return files;
          };

          const tgsFiles = await findTgsFiles(packExtractDir);
          console.log(`  Found ${tgsFiles.length} .tgs files`);

          if (tgsFiles.length === 0) {
            console.log(`  No .tgs files found, deleting pack`);
            await req.db.collection('ReadModel-StickerSetReadModel')
              .deleteOne({ StickerSetId: stickersetId });
            failedPacks.push({ pack: packTitle, reason: 'No .tgs files found' });
            continue;
          }

          // Загружаем все стикеры параллельно (пачками по 50)
          const packEmojis = new Map(); // emoji -> [documentIds]
          let uploadedCount = 0;
          const BATCH_SIZE = 50;

          console.log(`  Uploading in batches of ${BATCH_SIZE}...`);

          for (let i = 0; i < tgsFiles.length; i += BATCH_SIZE) {
            const batch = tgsFiles.slice(i, i + BATCH_SIZE);

            const uploadPromises = batch.map(async (tgsFile) => {
              try {
                const filename = path.basename(tgsFile);

                // Достаём эмодзи из имени файла: "🦆_12345.tgs" -> emoji="🦆"
                let emoji = '😀'; // значение по умолчанию
                const emojiMatch = filename.match(/^(.+?)_\d+\.tgs$/);
                if (emojiMatch) {
                  emoji = emojiMatch[1];
                }

                // Читаем и проверяем TGS
                const fileBuffer = await fs.readFile(tgsFile);

                try {
                  const decompressed = await gunzip(fileBuffer);
                  JSON.parse(decompressed.toString('utf8'));
                } catch (err) {
                  return { success: false, filename };
                }

                // Генерируем ID документа
                const documentId = Date.now() * 10000 + Math.floor(Math.random() * 10000);
                const docAccessHash = Date.now() * 100000 + Math.floor(Math.random() * 100000);

                // Загружаем в MinIO
                await minioHelper.uploadFile(tgsFile, documentId, 'application/x-tgsticker');

                // Создаём DocumentReadModel
                const documentReadModelId = `document-${documentId}`;
                const fileRefBuffer = Buffer.from(`${documentId}`);

                const documentReadModel = {
                  _id: documentReadModelId,
                  Id: documentReadModelId,
                  Version: 1,
                  DocumentId: documentId,
                  AccessHash: docAccessHash,
                  FileReference: fileRefBuffer,
                  DcId: 2,
                  Date: Math.floor(Date.now() / 1000),
                  MimeType: 'application/x-tgsticker',
                  Size: fileBuffer.length,
                  Name: `${documentId}`,
                  Md5CheckSum: null,
                  CreatorId: null,
                  Thumbs: null,
                  ThumbId: null,
                  VideoThumbs: null,
                  VideoThumbId: null,
                  Attributes: null,
                  Attributes2: null
                };

                await req.db.collection('ReadModel-DocumentReadModel').insertOne(documentReadModel);

                return { success: true, emoji, documentId };

              } catch (fileErr) {
                return { success: false, filename: path.basename(tgsFile), error: fileErr.message };
              }
            });

            // Wait for batch to complete
            const results = await Promise.all(uploadPromises);

            // Process results
            results.forEach(result => {
              if (result.success) {
                if (!packEmojis.has(result.emoji)) {
                  packEmojis.set(result.emoji, []);
                }
                packEmojis.get(result.emoji).push(result.documentId);
                uploadedCount++;
              }
            });

            console.log(`    Progress: ${uploadedCount}/${tgsFiles.length}`);
          }

          console.log(`  Uploaded ${uploadedCount}/${tgsFiles.length} stickers`);

          // Формируем массив Packs
          const packsArray = Array.from(packEmojis.entries()).map(([emoticon, documents]) => ({
            Emoticon: emoticon,
            Documents: documents
          }));

          // Собираем все ID документов
          const allDocumentIds = Array.from(packEmojis.values()).flat();

          // Обновляем пак всеми стикерами
          await req.db.collection('ReadModel-StickerSetReadModel').updateOne(
            { StickerSetId: stickersetId },
            {
              $set: {
                Count: uploadedCount,
                Packs: packsArray,
                StickerDocumentIds: allDocumentIds
              },
              $inc: { Version: 1 }
            }
          );

          createdPacks.push({
            title: packTitle,
            shortName,
            stickersetId,
            stickersCount: uploadedCount
          });

          totalStickersUploaded += uploadedCount;

        } catch (packErr) {
          console.error(`  Failed to process pack ${packZipName}:`, packErr);
          failedPacks.push({ pack: packZipName, reason: packErr.message });
        }
      }

      // Очистка временных файлов
      await fs.rm(mainExtractDir, { recursive: true, force: true });
      await fs.unlink(zipPath);

      console.log(`\n${'='.repeat(60)}`);
      console.log('REGULAR STICKER PACKS BULK UPLOAD COMPLETED');
      console.log(`${'='.repeat(60)}`);
      console.log(`Total packs created: ${createdPacks.length}`);
      console.log(`Total stickers uploaded: ${totalStickersUploaded}`);
      console.log(`Failed packs: ${failedPacks.length}\n`);

      res.json({
        success: true,
        totalPacks: packZips.length,
        createdPacks: createdPacks.length,
        failedPacks: failedPacks.length,
        totalStickers: totalStickersUploaded,
        packs: createdPacks,
        failures: failedPacks
      });

    } catch (err) {
      console.error('Error processing nested ZIP:', err);
      await fs.rm(mainExtractDir, { recursive: true, force: true }).catch(() => { });
      await fs.unlink(zipPath).catch(() => { });
      res.status(500).json({ error: err.message });
    }
  } catch (error) {
    console.error('Error in nested bulk upload:', error);
    res.status(500).json({ error: error.message });
  }
});



// Переключить статус рекомендуемого пака
router.post('/:stickerSetId/featured', async (req, res) => {
  try {
    const { stickerSetId } = req.params;
    const { isFeatured, featuredOrder } = req.body;

    await req.db.collection('ReadModel-StickerSetReadModel').updateOne(
      { StickerSetId: parseInt(stickerSetId) },
      {
        $set: {
          IsFeatured: isFeatured,
          FeaturedOrder: featuredOrder || 0
        }
      }
    );

    res.json({ success: true });
  } catch (error) {
    console.error('Error updating featured status:', error);
    res.status(500).json({ error: error.message });
  }
});

export default router;
