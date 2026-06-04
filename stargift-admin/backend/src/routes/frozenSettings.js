import express from 'express';
import multer from 'multer';
import path from 'path';
import fs from 'fs';
import { fileURLToPath } from 'url';
import { dirname } from 'path';
import { MongoClient } from 'mongodb';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const router = express.Router();

// Configure multer for TGS file upload
const storage = multer.diskStorage({
  destination: function (req, file, cb) {
    const uploadDir = path.join(__dirname, '../../uploads/frozen');
    if (!fs.existsSync(uploadDir)) {
      fs.mkdirSync(uploadDir, { recursive: true });
    }
    cb(null, uploadDir);
  },
  filename: function (req, file, cb) {
    cb(null, 'snowflake-' + Date.now() + path.extname(file.originalname));
  }
});

const upload = multer({
  storage: storage,
  fileFilter: function (req, file, cb) {
    // Accept TGS files
    if (file.mimetype === 'application/x-tgsticker' || 
        file.originalname.endsWith('.tgs') ||
        file.mimetype === 'application/gzip') {
      cb(null, true);
    } else {
      cb(new Error('Only TGS files are allowed!'), false);
    }
  },
  limits: {
    fileSize: 64 * 1024 // 64KB max for TGS
  }
});

/**
 * GET /api/frozen-settings
 * Get current frozen account settings
 */
router.get('/', async (req, res) => {
  try {
    const client = new MongoClient(process.env.MONGODB_URI || 'mongodb://localhost:27017');
    
    await client.connect();
    const db = client.db('tg');
    const settingsCollection = db.collection('settings');
    
    // Get frozen settings
    let settings = await settingsCollection.findOne({ _id: 'frozen-account-settings' });
    
    if (!settings) {
      settings = {
        _id: 'frozen-account-settings',
        snowflakeEmojiId: null,
        snowflakeFileName: null,
        snowflakeUploadedAt: null
      };
    }
    
    await client.close();
    
    res.json({
      success: true,
      settings: settings
    });
  } catch (error) {
    console.error('Error fetching frozen settings:', error);
    res.status(500).json({ success: false, error: error.message });
  }
});

/**
 * POST /api/frozen-settings/upload-snowflake
 * Upload TGS snowflake sticker
 */
router.post('/upload-snowflake', upload.single('snowflake'), async (req, res) => {
  try {
    if (!req.file) {
      return res.status(400).json({ success: false, error: 'No file uploaded' });
    }
    
    const client = new MongoClient(process.env.MONGODB_URI || 'mongodb://localhost:27017');
    
    await client.connect();
    const db = client.db('tg');
    const settingsCollection = db.collection('settings');
    
    // Save file info to settings
    const settings = {
      _id: 'frozen-account-settings',
      snowflakeFileName: req.file.filename,
      snowflakeFilePath: req.file.path,
      snowflakeUploadedAt: new Date(),
      snowflakeEmojiId: null // Will be set after uploading to MyTelegram
    };
    
    await settingsCollection.replaceOne(
      { _id: 'frozen-account-settings' },
      settings,
      { upsert: true }
    );
    
    await client.close();
    
    res.json({
      success: true,
      message: 'Snowflake TGS uploaded successfully',
      file: {
        filename: req.file.filename,
        size: req.file.size,
        path: req.file.path
      }
    });
  } catch (error) {
    console.error('Error uploading snowflake:', error);
    res.status(500).json({ success: false, error: error.message });
  }
});

/**
 * POST /api/frozen-settings/set-emoji-id
 * Set the Document ID for frozen snowflake emoji
 */
router.post('/set-emoji-id', async (req, res) => {
  try {
    const { documentId } = req.body;
    
    if (!documentId || isNaN(documentId)) {
      return res.status(400).json({ success: false, error: 'Valid documentId is required' });
    }
    
    const client = new MongoClient(process.env.MONGODB_URI || 'mongodb://localhost:27017');
    
    await client.connect();
    const db = client.db('tg');
    const settingsCollection = db.collection('settings');
    
    // Update emoji ID
    await settingsCollection.updateOne(
      { _id: 'frozen-account-settings' },
      { 
        $set: { 
          snowflakeEmojiId: parseInt(documentId),
          emojiIdUpdatedAt: new Date()
        } 
      },
      { upsert: true }
    );
    
    await client.close();
    
    res.json({
      success: true,
      message: 'Frozen snowflake emoji ID updated',
      documentId: parseInt(documentId)
    });
  } catch (error) {
    console.error('Error setting emoji ID:', error);
    res.status(500).json({ success: false, error: error.message });
  }
});

export default router;
