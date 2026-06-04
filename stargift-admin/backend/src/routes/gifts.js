import express from 'express';
import multer from 'multer';
import { promises as fs } from 'fs';
import path from 'path';
import Joi from 'joi';
import zlib from 'zlib';
import { promisify } from 'util';
import minioHelper from '../utils/minioHelper.js';

const gunzip = promisify(zlib.gunzip);
const router = express.Router();

// --- Configuration ---
const UPLOAD_DIR = 'uploads/stickers';
const MAX_FILE_SIZE = 10 * 1024 * 1024; // 10MB
const ALLOWED_MIME_TYPES = ['application/json', 'application/octet-stream', 'application/x-tgsticker'];
const ALLOWED_EXTENSIONS = ['.json', '.tgs'];

// --- Multer Setup ---
const storage = multer.diskStorage({
  destination: async (req, file, cb) => {
    await fs.mkdir(UPLOAD_DIR, { recursive: true });
    cb(null, UPLOAD_DIR);
  },
  filename: (req, file, cb) => {
    const uniqueSuffix = `${Date.now()}-${Math.round(Math.random() * 1E9)}`;
    cb(null, `sticker-${uniqueSuffix}${path.extname(file.originalname)}`);
  }
});

const upload = multer({
  storage,
  limits: { fileSize: MAX_FILE_SIZE },
  fileFilter: (req, file, cb) => {
    const ext = path.extname(file.originalname).toLowerCase();
    if (ALLOWED_EXTENSIONS.includes(ext)) {
      cb(null, true);
    } else {
      cb(new Error('Only .json and .tgs files are allowed'));
    }
  }
});

// --- Helpers ---
const toJSON = (data) => {
  return JSON.parse(JSON.stringify(data, (key, value) => {
    if (typeof value === 'bigint') return value.toString();
    if (value && typeof value === 'object' && (value._bsontype === 'Long' || (value.low !== undefined && value.high !== undefined))) {
      return value.toString();
    }
    return value;
  }));
};

const generateId = () => Date.now() * 10000 + Math.floor(Math.random() * 10000);
const generateAccessHash = () => Date.now() * 100000 + Math.floor(Math.random() * 100000);

// --- Validation ---
const giftSchema = Joi.object({
  giftId: Joi.alt([Joi.number().integer().positive(), Joi.string().pattern(/^\d+$/)]).required(),

  stars: Joi.number().integer().positive().required(),

  convertStars: Joi.number().integer().positive()
    .less(Joi.ref('stars'))
    .messages({
      'number.less': 'convertStars must be less than stars'
    })
    .required(),

  limited: Joi.boolean().default(false),
  soldOut: Joi.boolean().default(false),
  birthday: Joi.boolean().default(false),
  requirePremium: Joi.boolean().default(false),
  limitedPerUser: Joi.boolean().default(false),

  availabilityTotal: Joi.number().integer().positive().optional(),

  availabilityRemains: Joi.number().integer().min(0).max(Joi.ref('availabilityTotal')).optional(),

  perUserTotal: Joi.when('limitedPerUser', {
    is: true,
    then: Joi.number().integer().positive().required(),
    otherwise: Joi.forbidden()
  }),

  upgradeStars: Joi.number().integer().positive().optional().allow(null),
  title: Joi.string().max(100).optional().allow(null, ''),
  resellMinStars: Joi.number().integer().positive().optional().allow(null),

  stickerId: Joi.alt([Joi.number().integer().positive(), Joi.string().pattern(/^\d+$/)]).optional(),

  releasedBy: Joi.object({
    type: Joi.string().valid('user', 'channel').required(),
    id: Joi.number().integer().positive().required()
  }).optional().allow(null),

  // Эти поля не должны устанавливаться вручную
  firstSaleDate: Joi.forbidden(),
  lastSaleDate: Joi.forbidden()
});

// --- Routes ---

// GET All Gifts
router.get('/', async (req, res) => {
  try {
    const { soldOut, limited, sort = 'stars' } = req.query;
    const filter = {};
    if (soldOut !== undefined) filter.SoldOut = soldOut === 'true';
    if (limited !== undefined) filter.Limited = limited === 'true';

    const sortOptions = {};
    if (sort === 'stars') sortOptions.Stars = 1;
    else if (sort === 'date') sortOptions._id = -1;
    else if (sort === 'availability') sortOptions.AvailabilityRemains = 1;

    const gifts = await req.db.collection('AvailableStarGiftReadModel')
      .find(filter).sort(sortOptions).toArray();

    res.json(toJSON(gifts));
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// GET Gift by ID
router.get('/:giftId', async (req, res) => {
  try {
    const giftId = BigInt(req.params.giftId);
    const gift = await req.db.collection('AvailableStarGiftReadModel').findOne({ GiftId: giftId });
    if (!gift) return res.status(404).json({ error: 'Gift not found' });
    res.json(toJSON(gift));
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// POST Create Gift
router.post('/', upload.single('sticker'), async (req, res) => {
  let uploadedFilePath = null;
  try {
    const giftData = typeof req.body.data === 'string' ? JSON.parse(req.body.data) : req.body;
    const { error, value } = giftSchema.validate(giftData);
    if (error) return res.status(400).json({ error: error.details[0].message });

    const safeGiftId = BigInt(value.giftId);
    const existing = await req.db.collection('AvailableStarGiftReadModel').findOne({ GiftId: safeGiftId });
    if (existing) return res.status(409).json({ error: 'Gift with this ID already exists' });

    let stickerDocumentId = null;
    if (req.file) {
      uploadedFilePath = req.file.path;
      const fileBuffer = await fs.readFile(uploadedFilePath);

      // Проверяем TGS
      try {
        const decompressed = await gunzip(fileBuffer);
        JSON.parse(decompressed.toString('utf8'));
      } catch (e) {
        try {
          JSON.parse(fileBuffer.toString('utf8'));
        } catch (jsonErr) {
          throw new Error('Invalid TGS file');
        }
      }

      const documentId = generateId();
      const accessHash = generateAccessHash();

      await minioHelper.uploadFile(uploadedFilePath, documentId, 'application/x-tgsticker');

      // Правильный FileReference на 24 байта вместо 8
      const fileRefBuffer = Buffer.allocUnsafe(24);
      fileRefBuffer.writeUInt8(2, 0);  // версия 2
      fileRefBuffer.writeUInt8(2, 1);  // ID дата-центра
      fileRefBuffer.writeBigInt64BE(BigInt(documentId), 2);  // ID документа (8 байт)
      fileRefBuffer.writeBigInt64BE(BigInt(accessHash), 10); // access hash (8 байт)
      fileRefBuffer.writeUInt32BE(Math.floor(Date.now() / 1000), 18); // метка времени (4 байта)
      fileRefBuffer.writeUInt16BE(Math.floor(Math.random() * 0xFFFF), 22); // случайные данные (2 байта)

      const documentReadModel = {
        _id: `document-${documentId}`,
        Id: `document-${documentId}`,
        DocumentId: documentId,
        Version: 1,
        AccessHash: accessHash,
        FileReference: fileRefBuffer,
        DcId: 2,
        Date: Math.floor(Date.now() / 1000),
        MimeType: 'application/x-tgsticker',
        Size: fileBuffer.length,
        Name: `${documentId}`,
        Md5CheckSum: null,
        CreatorId: null,
        Thumbs: null, // null вместо хардкода — Telegram сгенерирует сам
        ThumbId: null,
        VideoThumbs: null,
        VideoThumbId: null,
        Attributes: null,
        Attributes2: [
          { _t: 'TDocumentAttributeImageSize', W: 512, H: 512 },
          {
            _t: 'TDocumentAttributeSticker',
            Alt: '🎁',
            Stickerset: {
              _t: 'TInputStickerSetEmpty'
            },
            Mask: false
          },
          { _t: 'TDocumentAttributeAnimated' },
          { _t: 'TDocumentAttributeFilename', FileName: 'sticker.tgs' }
        ]
      };

      await req.db.collection('ReadModel-DocumentReadModel').insertOne(documentReadModel);
      stickerDocumentId = documentId;
      await fs.unlink(uploadedFilePath);
      uploadedFilePath = null;
    } else if (value.stickerId) {
      stickerDocumentId = BigInt(value.stickerId);
    }

    const gift = {
      _id: value.giftId.toString(),
      GiftId: safeGiftId,
      Limited: value.limited,
      SoldOut: value.soldOut,
      Birthday: value.birthday,
      RequirePremium: value.requirePremium || false, // берём из value
      LimitedPerUser: value.limitedPerUser || false, // берём из value
      Stars: value.stars,
      ConvertStars: value.convertStars,
      UpgradeStars: value.upgradeStars || null,
      AvailabilityTotal: value.availabilityTotal || null,
      AvailabilityRemains: value.availabilityRemains ?? value.availabilityTotal ?? null,
      AvailabilityResale: null,
      FirstSaleDate: null, // null — установится при первой покупке
      LastSaleDate: null,  // null — установится при покупке
      Title: value.title || null, // null вместо пустой строки
      ResellMinStars: value.resellMinStars || null,
      Sticker: stickerDocumentId,
      ReleasedBy: value.releasedBy ? { // поддержка ReleasedBy
        Type: value.releasedBy.type,
        Id: value.releasedBy.id
      } : null,
      PerUserTotal: value.limitedPerUser ? value.perUserTotal : null, // берём из value
      PerUserRemains: value.limitedPerUser ? value.perUserTotal : null, // берём из value
      LockedUntilDate: null,
      Version: 1
    };

    await req.db.collection('AvailableStarGiftReadModel').insertOne(gift);
    res.status(201).json(toJSON(gift));

  } catch (error) {
    console.error('Create gift error:', error);
    if (uploadedFilePath) await fs.unlink(uploadedFilePath).catch(() => { });
    res.status(500).json({ error: error.message });
  }
});

// PUT Update Gift
router.put('/:giftId', async (req, res) => {
  try {
    const giftId = BigInt(req.params.giftId);
    const giftData = req.body;
    delete giftData.giftId;

    const result = await req.db.collection('AvailableStarGiftReadModel').findOneAndUpdate(
      { GiftId: giftId },
      { $set: { ...giftData, UpdatedAt: new Date() } },
      { returnDocument: 'after' }
    );

    if (!result) return res.status(404).json({ error: 'Gift not found' });
    res.json(toJSON(result));
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// DELETE Gift
router.delete('/:giftId', async (req, res) => {
  try {
    const giftId = BigInt(req.params.giftId);
    const result = await req.db.collection('AvailableStarGiftReadModel').deleteOne({ GiftId: giftId });
    if (result.deletedCount === 0) return res.status(404).json({ error: 'Gift not found' });
    res.json({ message: 'Gift deleted' });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// POST Send Gift to User (Admin/Test)
router.post('/:giftId/send-to-user', async (req, res) => {
  try {
    const giftId = BigInt(req.params.giftId);
    const { userId, fromUserId, message, nameHidden, count } = req.body;
    if (!userId) return res.status(400).json({ error: 'userId is required' });

    const gift = await req.db.collection('AvailableStarGiftReadModel').findOne({ GiftId: giftId });
    if (!gift) return res.status(404).json({ error: 'Gift not found' });

    const fromId = fromUserId ? Number(fromUserId) : 0;
    if (!fromId || fromId <= 0) return res.status(400).json({ success: false, error: 'fromUserId is required (stars will be spent)' });

    const payload = {
      fromUserId: fromId,
      toUserId: parseInt(userId),
      giftId: Number(giftId),
      count: count || 1,
      nameHidden: !!nameHidden,
      canUpgrade: null,
      message: message || null
    };

    const adminUrl = process.env.ADMIN_API_URL || 'http://localhost:5000';
    const adminKey = process.env.ADMIN_API_KEY || '';

    const response = await fetch(`${adminUrl}/api/admin/star-gifts/send`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-Admin-API-Key': adminKey
      },
      body: JSON.stringify(payload)
    });

    const rawText = await response.text();
    let data = {};
    try {
      data = rawText ? JSON.parse(rawText) : {};
    } catch {
      data = { error: rawText };
    }

    if (!response.ok) {
      console.error('Send gift error:', { status: response.status, body: data });
      return res.status(response.status).json(data);
    }

    res.status(200).json({ success: true, ...data });
  } catch (error) {
    console.error('Send gift error:', error);
    res.status(500).json({ error: error.message });
  }
});

export default router;
