import express from 'express';
import multer from 'multer';
import { promises as fs } from 'fs';
import path from 'path';
import Joi from 'joi';
import * as minioHelper from '../utils/minioHelper.js';

const router = express.Router();

// Настройка загрузки файлов для иконок TGS
const storage = multer.diskStorage({
  destination: async (req, file, cb) => {
    const uploadDir = 'uploads/verification-icons';
    await fs.mkdir(uploadDir, { recursive: true });
    cb(null, uploadDir);
  },
  filename: (req, file, cb) => {
    const uniqueSuffix = Date.now() + '-' + Math.round(Math.random() * 1E9);
    cb(null, `verification-icon-${uniqueSuffix}${path.extname(file.originalname)}`);
  }
});

const upload = multer({
  storage,
  limits: { fileSize: 5 * 1024 * 1024 }, // 5MB
  fileFilter: (req, file, cb) => {
    const allowedTypes = ['application/json', 'application/octet-stream', 'application/x-tgsticker'];
    const allowedExts = ['.json', '.tgs'];
    const ext = path.extname(file.originalname).toLowerCase();
    
    if (allowedExts.includes(ext)) {
      cb(null, true);
    } else {
      cb(new Error('Only .json and .tgs files are allowed'));
    }
  }
});

// Схемы валидации
const botVerifierSchema = Joi.object({
  botUserId: Joi.number().integer().positive().required(),
  iconEmojiId: Joi.number().integer().positive().required(),
  companyName: Joi.string().min(1).max(200).required(),
  canModifyCustomDescription: Joi.boolean().default(false)
});

const verificationSchema = Joi.object({
  targetUserId: Joi.number().integer().positive().required(),
  botUserId: Joi.number().integer().positive().optional(),
  customDescription: Joi.string().max(500).optional().allow(null, '')
});

// GET /api/verification/bots - Get all bot verifiers
router.get('/bots', async (req, res) => {
  try {
    const verifiers = await req.db
      .collection('ReadModel-BotVerifierReadModel')
      .find({ IsActive: true })
      .toArray();
    
    res.json({
      success: true,
      data: verifiers
    });
  } catch (error) {
    console.error('Error fetching bot verifiers:', error);
    res.status(500).json({
      success: false,
      error: 'Failed to fetch bot verifiers'
    });
  }
});

// GET /api/verification/bots/:botUserId - Get specific bot verifier
router.get('/bots/:botUserId', async (req, res) => {
  try {
    const { botUserId } = req.params;
    
    const verifier = await req.db
      .collection('ReadModel-BotVerifierReadModel')
      .findOne({ BotUserId: parseInt(botUserId), IsActive: true });
    
    if (!verifier) {
      return res.status(404).json({
        success: false,
        error: 'Bot verifier not found'
      });
    }
    
    res.json({
      success: true,
      data: verifier
    });
  } catch (error) {
    console.error('Error fetching bot verifier:', error);
    res.status(500).json({
      success: false,
      error: 'Failed to fetch bot verifier'
    });
  }
});

// POST /api/verification/bots - Create new bot verifier
router.post('/bots', async (req, res) => {
  try {
    const { error, value } = botVerifierSchema.validate(req.body);
    
    if (error) {
      return res.status(400).json({
        success: false,
        error: error.details[0].message
      });
    }
    
    // Check if bot user exists
    const botUser = await req.db
      .collection('eventflow-userreadmodel')
      .findOne({ UserId: value.botUserId, Bot: true });
    
    if (!botUser) {
      return res.status(404).json({
        success: false,
        error: 'Bot user not found'
      });
    }
    
    // Check if verifier already exists
    const existing = await req.db
      .collection('ReadModel-BotVerifierReadModel')
      .findOne({ BotUserId: value.botUserId });
    
    const verifier = {
      Id: `bot_${value.botUserId}`,
      BotUserId: value.botUserId,
      IconEmojiId: value.iconEmojiId,
      CompanyName: value.companyName,
      CanModifyCustomDescription: value.canModifyCustomDescription,
      IsActive: true,
      CreatedAt: existing?.CreatedAt || new Date(),
      UpdatedAt: new Date()
    };
    
    if (existing) {
      // Update existing verifier
      await req.db
        .collection('ReadModel-BotVerifierReadModel')
        .updateOne(
          { BotUserId: value.botUserId },
          { $set: verifier }
        );
    } else {
      // Create new verifier
      await req.db
        .collection('ReadModel-BotVerifierReadModel')
        .insertOne(verifier);
    }
    
    res.status(201).json({
      success: true,
      data: verifier
    });
  } catch (error) {
    console.error('Error creating bot verifier:', error);
    res.status(500).json({
      success: false,
      error: 'Failed to create bot verifier'
    });
  }
});

// PUT /api/verification/bots/:botUserId - Update bot verifier
router.put('/bots/:botUserId', async (req, res) => {
  try {
    const { botUserId } = req.params;
    const { error, value } = botVerifierSchema.validate(req.body);
    
    if (error) {
      return res.status(400).json({
        success: false,
        error: error.details[0].message
      });
    }
    
    const result = await req.db
      .collection('ReadModel-BotVerifierReadModel')
      .findOneAndUpdate(
        { BotUserId: parseInt(botUserId) },
        {
          $set: {
            IconEmojiId: value.iconEmojiId,
            CompanyName: value.companyName,
            CanModifyCustomDescription: value.canModifyCustomDescription,
            UpdatedAt: new Date()
          }
        },
        { returnDocument: 'after' }
      );
    
    if (!result) {
      return res.status(404).json({
        success: false,
        error: 'Bot verifier not found'
      });
    }
    
    res.json({
      success: true,
      data: result
    });
  } catch (error) {
    console.error('Error updating bot verifier:', error);
    res.status(500).json({
      success: false,
      error: 'Failed to update bot verifier'
    });
  }
});

// DELETE /api/verification/bots/:botUserId - Deactivate bot verifier
router.delete('/bots/:botUserId', async (req, res) => {
  try {
    const { botUserId } = req.params;
    
    const result = await req.db
      .collection('ReadModel-BotVerifierReadModel')
      .findOneAndUpdate(
        { BotUserId: parseInt(botUserId) },
        {
          $set: {
            IsActive: false,
            UpdatedAt: new Date()
          }
        },
        { returnDocument: 'after' }
      );
    
    if (!result) {
      return res.status(404).json({
        success: false,
        error: 'Bot verifier not found'
      });
    }
    
    res.json({
      success: true,
      data: result
    });
  } catch (error) {
    console.error('Error deactivating bot verifier:', error);
    res.status(500).json({
      success: false,
      error: 'Failed to deactivate bot verifier'
    });
  }
});

// GET /api/verification/users/:userId - Get user verification status
router.get('/users/:userId', async (req, res) => {
  try {
    const { userId } = req.params;
    
    const verification = await req.db
      .collection('ReadModel-BotVerificationReadModel')
      .findOne({ TargetType: 1, TargetId: parseInt(userId) });
    
    if (!verification) {
      return res.json({
        success: true,
        data: null
      });
    }
    
    res.json({
      success: true,
      data: verification
    });
  } catch (error) {
    console.error('Error fetching user verification:', error);
    res.status(500).json({
      success: false,
      error: 'Failed to fetch verification status'
    });
  }
});

// POST /api/verification/users/:userId - Set user verification
router.post('/users/:userId', async (req, res) => {
  try {
    const { userId } = req.params;
    const { error, value } = verificationSchema.validate({
      ...req.body,
      targetUserId: parseInt(userId)
    });
    
    if (error) {
      return res.status(400).json({
        success: false,
        error: error.details[0].message
      });
    }
    
    // Check if target user exists
    const targetUser = await req.db
      .collection('eventflow-userreadmodel')
      .findOne({ UserId: value.targetUserId });
    
    if (!targetUser) {
      return res.status(404).json({
        success: false,
        error: 'User not found'
      });
    }
    
    // Get bot verifier
    const botUserId = value.botUserId || req.body.botUserId;
    const verifier = await req.db
      .collection('ReadModel-BotVerifierReadModel')
      .findOne({ BotUserId: botUserId, IsActive: true });
    
    if (!verifier) {
      return res.status(404).json({
        success: false,
        error: 'Bot verifier not found or inactive'
      });
    }
    
    // Проверяем право задавать собственное описание
    if (value.customDescription && !verifier.CanModifyCustomDescription) {
      return res.status(403).json({
        success: false,
        error: 'Bot verifier cannot modify custom description'
      });
    }

    // По умолчанию описание берётся из CompanyName.
    // Если задано непустое CustomDescription, оно заменяет описание по умолчанию.
    const defaultDescription = verifier.CompanyName;
    const customDescription = value.customDescription && value.customDescription.trim() !== '' 
      ? value.customDescription.trim() 
      : null;
    
    // Update UserReadModel with verification (both collections for compatibility)
    const updateData = {
      BotVerifierId: parseInt(verifier.BotUserId),
      BotVerificationIcon: parseInt(verifier.IconEmojiId)
    };
    
    // Check if UserReadModel exists
    const existingUser = await req.db.collection('ReadModel-UserReadModel').findOne({ UserId: value.targetUserId });
    
    if (!existingUser) {
      console.log(`UserReadModel does not exist for user ${value.targetUserId}`);
      console.log(`UserReadModel создаётся Command Server при регистрации или обновлении пользователя`);
      console.log(`Пока создаём минимальный UserReadModel только с полями верификации`);

      // Создаём минимальный UserReadModel с полями верификации
      await req.db.collection('ReadModel-UserReadModel').insertOne({
        _id: `user-${value.targetUserId}`,
        Id: `user-${value.targetUserId}`,
        UserId: value.targetUserId,
        BotVerifierId: parseInt(verifier.BotUserId),
        BotVerificationIcon: parseInt(verifier.IconEmojiId),
        Version: 1
      });
      console.log(`Created minimal UserReadModel for user ${value.targetUserId}`);
    } else {
      // Обновляем существующий UserReadModel и увеличиваем Version, чтобы сбросить кэш
      const currentVersion = existingUser.Version || 1;
      await req.db.collection('ReadModel-UserReadModel').updateOne(
        { UserId: value.targetUserId },
        {
          $set: updateData,
          $inc: { Version: 1 }  // увеличиваем версию, чтобы сбросить кэш Query Server
        }
      );
      console.log(`Updated existing UserReadModel for user ${value.targetUserId} (Version: ${currentVersion} -> ${currentVersion + 1})`);
    }

    // Также обновляем коллекцию eventflow
    await req.db.collection('eventflow-userreadmodel').updateOne(
      { UserId: value.targetUserId },
      { $set: updateData },
      { upsert: true }
    );
    
    // Создаём или обновляем BotVerificationReadModel
    const verificationId = `verification_1_${value.targetUserId}`;  // формат: verification_{TargetType}_{TargetId}
    const verification = {
      _id: verificationId,  // явно задаём _id строкой
      Id: verificationId,
      TargetType: 1, // пользователь
      TargetId: value.targetUserId,
      BotVerifierId: verifier.BotUserId,  // используем BotVerifierId (имя поля в C#-ReadModel)
      IconEmojiId: verifier.IconEmojiId,
      Description: defaultDescription,  // всегда задаём описание по умолчанию из CompanyName
      CustomDescription: customDescription,  // null или собственный текст
      VerifiedAt: new Date(),
      UpdatedAt: new Date()
    };

    console.log(`Creating verification: Description="${defaultDescription}", CustomDescription=${customDescription ? `"${customDescription}"` : 'null'}`);

    // Удаляем старую запись, если она есть (чтобы избежать конфликта _id)
    await req.db.collection('ReadModel-BotVerificationReadModel').deleteOne({
      TargetType: 1,
      TargetId: value.targetUserId
    });

    // Вставляем новую запись
    await req.db.collection('ReadModel-BotVerificationReadModel').insertOne(verification);

    // Сбрасываем кэш UserReadModel, обновляя служебное поле:
    // это заставляет Query Server перечитать пользователя из MongoDB
    await req.db.collection('ReadModel-UserReadModel').updateOne(
      { UserId: value.targetUserId },
      { $set: { _cacheInvalidated: new Date() } }
    );

    console.log(`Verification created successfully for user ${value.targetUserId}`);
    
    res.json({
      success: true,
      data: verification,
      warning: 'Please restart messenger-query-server to see changes: docker-compose restart messenger-query-server'
    });
  } catch (error) {
    console.error('Error setting user verification:', error);
    res.status(500).json({
      success: false,
      error: 'Failed to set user verification'
    });
  }
});

// GET /api/verification/users/:userId/debug - отладочные данные о верификации пользователя
router.get('/users/:userId/debug', async (req, res) => {
  try {
    const userId = parseInt(req.params.userId);

    // Проверяем UserReadModel
    const userReadModel = await req.db.collection('ReadModel-UserReadModel').findOne({ UserId: userId });

    // Проверяем BotVerificationReadModel
    const verification = await req.db.collection('ReadModel-BotVerificationReadModel').findOne({
      TargetType: 1,
      TargetId: userId
    });
    
    res.json({
      success: true,
      data: {
        userReadModel: {
          BotVerifierId: userReadModel?.BotVerifierId || null,
          BotVerificationIcon: userReadModel?.BotVerificationIcon || null
        },
        verification: verification || null
      }
    });
  } catch (error) {
    console.error('Error debugging verification:', error);
    res.status(500).json({ success: false, error: error.message });
  }
});

// DELETE /api/verification/users/:userId - Remove user verification
router.delete('/users/:userId', async (req, res) => {
  try {
    const { userId } = req.params;
    
    // Убираем верификацию из UserReadModel (в обеих коллекциях)
    const unsetData = {
      BotVerifierId: "",
      BotVerificationIcon: ""
    };

    // Запоминаем текущую версию до обновления
    const existingUser = await req.db.collection('ReadModel-UserReadModel').findOne({ UserId: parseInt(userId) });
    const currentVersion = existingUser?.Version || 1;

    await Promise.all([
      req.db.collection('eventflow-userreadmodel').updateOne(
        { UserId: parseInt(userId) },
        { $unset: unsetData }
      ),
      req.db.collection('ReadModel-UserReadModel').updateOne(
        { UserId: parseInt(userId) },
        {
          $unset: unsetData,
          $inc: { Version: 1 }  // увеличиваем версию, чтобы сбросить кэш Query Server
        }
      )
    ]);

    console.log(`Removed verification for user ${userId} (Version: ${currentVersion} -> ${currentVersion + 1})`);

    // Удаляем из BotVerificationReadModel
    await req.db.collection('ReadModel-BotVerificationReadModel').deleteOne({
      TargetType: 1,
      TargetId: parseInt(userId)
    });
    
    res.json({
      success: true,
      message: 'Verification removed successfully'
    });
  } catch (error) {
    console.error('Error removing user verification:', error);
    res.status(500).json({
      success: false,
      error: 'Failed to remove verification'
    });
  }
});

// POST /api/verification/upload-icon - загрузка файла иконки TGS
router.post('/upload-icon', upload.single('icon'), async (req, res) => {
  try {
    if (!req.file) {
      return res.status(400).json({
        success: false,
        error: 'No file uploaded'
      });
    }
    
    const filePath = req.file.path;
    const ext = path.extname(req.file.originalname).toLowerCase();
    
    if (ext !== '.tgs') {
      await fs.unlink(filePath);
      return res.status(400).json({
        success: false,
        error: 'Only .tgs files are allowed'
      });
    }
    
    // Генерируем уникальный ID документа для этого эмодзи
    const documentId = Date.now() * 1000 + Math.floor(Math.random() * 1000);

    // Загружаем в MinIO (как для эмодзи-паков)
    await minioHelper.uploadFile(filePath, documentId, 'application/x-tgsticker');

    // Создаём DocumentReadModel (та же структура, что и у эмодзи-паков)
    const fileReference = Buffer.alloc(8);
    fileReference.writeBigInt64LE(BigInt(documentId));
    
    const documentReadModel = {
      _id: `document-${documentId}`,
      Id: `document-${documentId}`,
      Version: 1,
      DocumentId: documentId,
      AccessHash: documentId + 100,
      FileReference: fileReference,
      DcId: 2,
      Date: Math.floor(Date.now() / 1000),
      MimeType: 'application/x-tgsticker',
      Size: req.file.size,
      Name: String(documentId),
      Attributes2: [
        {
          _t: 'TDocumentAttributeCustomEmoji',
          Free: true,
          TextColor: false,
          Alt: '🏆',
          Stickerset: {
            _t: 'TInputStickerSetEmpty'
          }
        }
      ]
    };
    
    // Используем правильное имя коллекции (соглашение об именовании EventFlow)
    const collectionName = 'ReadModel-DocumentReadModel';

    const insertResult = await req.db
      .collection(collectionName)
      .insertOne(documentReadModel);

    console.log(`DocumentReadModel created in ${collectionName}: documentId=${documentId}, insertedId=${insertResult.insertedId}`);

    // Убеждаемся, что документ создан
    const verifyDoc = await req.db
      .collection(collectionName)
      .findOne({ DocumentId: documentId });

    if (!verifyDoc) {
      console.error(`Document not found after insert: documentId=${documentId}`);
      throw new Error('Document verification failed');
    }

    console.log(`Document verified in MongoDB: documentId=${documentId}`);

    // Удаляем загруженный файл
    await fs.unlink(filePath);
    
    res.json({
      success: true,
      data: {
        documentId,
        size: req.file.size
      }
    });
  } catch (error) {
    console.error('Error uploading icon:', error);
    if (req.file) {
      await fs.unlink(req.file.path).catch(() => {});
    }
    res.status(500).json({
      success: false,
      error: 'Failed to upload icon'
    });
  }
});

// GET /api/verification/stats - статистика верификации
router.get('/stats', async (req, res) => {
  try {
    const [totalVerifiers, activeVerifiers, totalVerifications, userVerifications] = await Promise.all([
      req.db.collection('ReadModel-BotVerifierReadModel').countDocuments({}),
      req.db.collection('ReadModel-BotVerifierReadModel').countDocuments({ IsActive: true }),
      req.db.collection('ReadModel-BotVerificationReadModel').countDocuments({}),
      req.db.collection('ReadModel-BotVerificationReadModel').countDocuments({ TargetType: 1 })
    ]);
    
    res.json({
      success: true,
      data: {
        totalVerifiers,
        activeVerifiers,
        totalVerifications,
        userVerifications,
        channelVerifications: totalVerifications - userVerifications
      }
    });
  } catch (error) {
    console.error('Error fetching verification stats:', error);
    res.status(500).json({
      success: false,
      error: 'Failed to fetch statistics'
    });
  }
});

// GET /api/verification/icons/:documentId/check - проверить наличие документа иконки
router.get('/icons/:documentId/check', async (req, res) => {
  try {
    const documentId = parseInt(req.params.documentId);

    // Проверяем в MongoDB
    const document = await req.db
      .collection('ReadModel-DocumentReadModel')
      .findOne({ DocumentId: documentId });

    // Проверяем в MinIO
    const fileExistsInMinio = await minioHelper.fileExists(documentId);
    
    res.json({
      success: true,
      data: {
        documentId,
        existsInMongoDB: !!document,
        existsInMinIO: fileExistsInMinio,
        document: document ? {
          DocumentId: document.DocumentId,
          Size: document.Size,
          MimeType: document.MimeType,
          DcId: document.DcId
        } : null
      }
    });
  } catch (error) {
    console.error('Error checking icon:', error);
    res.status(500).json({
      success: false,
      error: 'Failed to check icon'
    });
  }
});

export default router;
