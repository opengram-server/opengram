import dotenv from 'dotenv';
import { fileURLToPath } from 'url';
import { dirname, join } from 'path';

// Определяем текущий каталог (в ES-модулях нет __dirname по умолчанию)
const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

// Загружаем переменные окружения в первую очередь, до остальных импортов
const envPath = join(__dirname, '..', '.env');
dotenv.config({ path: envPath });

// Выводим в лог, откуда загружен .env, и проверяем ключи MinIO
console.log('Loading .env from:', envPath);
console.log('MINIO_ACCESS_KEY:', process.env.MINIO_ACCESS_KEY || 'NOT SET');
console.log('MINIO_SECRET_KEY:', process.env.MINIO_SECRET_KEY ? '***' + process.env.MINIO_SECRET_KEY.slice(-4) : 'NOT SET');

import express from 'express';
import cors from 'cors';
import { MongoClient } from 'mongodb';
import giftsRouter from './routes/gifts.js';
import attributesRouter from './routes/attributes.js';
import statsRouter from './routes/stats.js';
import emojiPacksRouter from './routes/emojipacks.js';
import stickerPacksRouter from './routes/stickerpacks.js';
import verificationRouter from './routes/verification.js';
import reactionsRouter from './routes/reactions.js';
import usersRouter from './routes/users.js';
import sponsoredMessagesRouter from './routes/sponsoredMessages.js';
import serviceNotificationsRouter from './routes/serviceNotifications.js';
import settingsRouter from './routes/settings.js';
import uploadRouter from './routes/upload.js';
import starsRouter from './routes/stars.js';
import frozenAccountsRouter from './routes/frozenAccounts.js';
import frozenSettingsRouter from './routes/frozenSettings.js';
import minioHelper from './utils/minioHelper.js';

const app = express();
const PORT = process.env.PORT || 3001;

// Middleware
app.use(cors({
  origin: process.env.CORS_ORIGIN || 'http://localhost:5173'
}));
app.use(express.json({ limit: '500mb' }));
app.use(express.urlencoded({ limit: '500mb', extended: true }));
app.use('/uploads', express.static('uploads'));

// MongoDB Connection
let db;
const mongoClient = new MongoClient(process.env.MONGODB_URI || 'mongodb://localhost:27017');

async function connectDB() {
  try {
    await mongoClient.connect();
    db = mongoClient.db(process.env.DB_NAME || 'tg');
    console.log('Connected to MongoDB');

    // Создаём индексы
    await db.collection('AvailableStarGiftReadModel').createIndex({ GiftId: 1 }, { unique: true });
    await db.collection('AvailableStarGiftReadModel').createIndex({ SoldOut: 1, Stars: 1 });
    await db.collection('ReadModel-UserReadModel').createIndex({ UserId: 1 });
    await db.collection('ReadModel-UserReadModel').createIndex({ UserName: 1 });
    await db.collection('ReadModel-UserReadModel').createIndex({ PhoneNumber: 1 });

    // Индексы для кастомных эмодзи
    await db.collection('custom_emoji_documents').createIndex({ document_id: 1 }, { unique: true });
    await db.collection('custom_emoji_documents').createIndex({ 'attributes.stickerset_id': 1 });
    await db.collection('custom_emoji_documents').createIndex({ 'attributes.alt': 1 });
    // Коллекции read-моделей EventFlow
    await db.collection('ReadModel-StickerSetReadModel').createIndex({ StickerSetId: 1 }, { unique: true });
    await db.collection('ReadModel-StickerSetReadModel').createIndex({ ShortName: 1 }, { unique: true });
    await db.collection('ReadModel-DocumentReadModel').createIndex({ DocumentId: 1 }, { unique: true });
    await db.collection('ReadModel-DocumentReadModel').createIndex({ _id: 1 });
    await db.collection('emoji_packs').createIndex({ stickerset_id: 1, emoticon: 1 }, { unique: true });

    // Индексы для рекламных сообщений
    await db.collection('ReadModel-SponsoredMessageReadModel').createIndex({ Id: 1 }, { unique: true });
    await db.collection('ReadModel-SponsoredMessageReadModel').createIndex({ ChannelId: 1, IsActive: 1 });
    await db.collection('ReadModel-SponsoredMessageReadModel').createIndex({ CreatedDate: -1 });

    console.log('Database indexes created');

    // Инициализируем bucket в MinIO
    try {
      await minioHelper.ensureBucket();
      console.log('MinIO storage initialized');
    } catch (error) {
      console.warn('MinIO initialization failed (will retry on upload):', error.message);
    }
  } catch (error) {
    console.error('MongoDB connection error:', error);
    process.exit(1);
  }
}

// Прокидываем подключение к БД во все маршруты
app.use((req, res, next) => {
  req.db = db;
  next();
});

// Routes
app.use('/api/gifts', giftsRouter);
app.use('/api/attributes', attributesRouter);
app.use('/api/stats', statsRouter);
app.use('/api/emojipacks', emojiPacksRouter);
app.use('/api/stickerpacks', stickerPacksRouter);
app.use('/api/verification', verificationRouter);
app.use('/api/reactions', reactionsRouter);
app.use('/api/users', usersRouter);
app.use('/api/sponsored-messages', sponsoredMessagesRouter);
app.use('/api/service-notifications', serviceNotificationsRouter);
app.use('/api/users/settings', settingsRouter);
app.use('/api/upload', uploadRouter);
app.use('/api/stars', starsRouter);
app.use('/api/frozen-accounts', frozenAccountsRouter);
app.use('/api/frozen-settings', frozenSettingsRouter);

// Быстрый поиск пользователя (устаревший вариант, перенесён в роутер users)
app.get('/api/users-legacy/search', async (req, res) => {
  try {
    const { phone, username, limit = 10 } = req.query;

    const filter = {};
    if (phone) filter.PhoneNumber = phone;
    if (username) filter.UserName = { $regex: username, $options: 'i' };

    const users = await db
      .collection('ReadModel-UserReadModel')
      .find(filter)
      .limit(parseInt(limit))
      .project({ UserId: 1, PhoneNumber: 1, UserName: 1, FirstName: 1, LastName: 1 })
      .toArray();

    res.json(users);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Список всех пользователей с постраничной разбивкой
app.get('/api/users', async (req, res) => {
  try {
    const { page = 1, limit = 50, search = '' } = req.query;
    const skip = (parseInt(page) - 1) * parseInt(limit);

    // Формируем фильтр поиска
    const filter = {};
    if (search) {
      filter.$or = [
        { FirstName: { $regex: search, $options: 'i' } },
        { LastName: { $regex: search, $options: 'i' } },
        { UserName: { $regex: search, $options: 'i' } },
        { PhoneNumber: { $regex: search, $options: 'i' } }
      ];
    }

    const [users, total] = await Promise.all([
      db.collection('ReadModel-UserReadModel')
        .find(filter)
        .sort({ UserId: -1 }) // Сначала новые
        .skip(skip)
        .limit(parseInt(limit))
        .project({
          UserId: 1,
          PhoneNumber: 1,
          UserName: 1,
          FirstName: 1,
          LastName: 1,
          AccessHash: 1,
          IsDeleted: 1,
          Bot: 1
        })
        .toArray(),
      db.collection('ReadModel-UserReadModel').countDocuments(filter)
    ]);

    res.json({
      users,
      pagination: {
        page: parseInt(page),
        limit: parseInt(limit),
        total,
        totalPages: Math.ceil(total / parseInt(limit))
      }
    });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Список всех каналов
app.get('/api/channels', async (req, res) => {
  try {
    const channels = await db.collection('eventflow-channelreadmodel')
      .find({ IsDeleted: { $ne: true } })
      .sort({ ChannelId: -1 })
      .project({
        ChannelId: 1,
        Title: 1,
        UserName: 1,
        ParticipantsCount: 1,
        AccessHash: 1,
        Broadcast: 1,
        MegaGroup: 1
      })
      .toArray();

    // Переименовываем ParticipantsCount в MembersCount для совместимости с фронтендом
    const formattedChannels = channels.map(ch => ({
      ...ch,
      MembersCount: ch.ParticipantsCount || 0
    }));

    res.json({
      success: true,
      channels: formattedChannels
    });
  } catch (error) {
    console.error('Error fetching channels:', error);
    res.status(500).json({
      success: false,
      error: error.message
    });
  }
});

// Проверка работоспособности сервиса
app.get('/api/health', (req, res) => {
  res.json({
    status: 'ok',
    mongodb: db ? 'connected' : 'disconnected',
    timestamp: new Date().toISOString()
  });
});

// Эндпоинт для проверки соединения с MinIO
app.get('/api/test/minio', async (req, res) => {
  try {
    const { Client } = await import('minio');

    // Берём конфигурацию из переменных окружения
    const config = {
      endPoint: process.env.MINIO_ENDPOINT || 'localhost',
      port: parseInt(process.env.MINIO_PORT || '9000'),
      useSSL: false,
      accessKey: process.env.MINIO_ACCESS_KEY || 'minioadmin',
      secretKey: process.env.MINIO_SECRET_KEY || 'minioadmin'
    };

    const testClient = new Client(config);
    const bucketName = 'tg-files';

    // Проверяем соединение, запросив список bucket'ов
    const startTime = Date.now();
    let buckets;
    try {
      buckets = await testClient.listBuckets();
    } catch (listError) {
      throw new Error(`Failed to list buckets: ${listError.message}`);
    }
    const listTime = Date.now() - startTime;

    // Проверяем, существует ли нужный нам bucket
    const bucketExists = buckets.some(b => b.name === bucketName);

    // Если bucket'а нет, пробуем его создать
    let bucketCreated = false;
    if (!bucketExists) {
      try {
        await testClient.makeBucket(bucketName, 'us-east-1');
        bucketCreated = true;
      } catch (createError) {
        // Не критично, просто отражаем это в ответе
      }
    }

    res.json({
      success: true,
      config: {
        endPoint: config.endPoint,
        port: config.port,
        useSSL: config.useSSL,
        accessKey: config.accessKey,
        secretKey: '***' + config.secretKey.slice(-4)
      },
      connection: {
        status: 'connected',
        responseTime: `${listTime}ms`
      },
      buckets: buckets.map(b => ({
        name: b.name,
        creationDate: b.creationDate
      })),
      targetBucket: {
        name: bucketName,
        exists: bucketExists,
        created: bucketCreated
      },
      timestamp: new Date().toISOString()
    });
  } catch (error) {
    res.status(500).json({
      success: false,
      error: error.message,
      stack: error.stack,
      config: {
        endPoint: process.env.MINIO_ENDPOINT || 'localhost',
        port: process.env.MINIO_PORT || '9000',
        accessKey: process.env.MINIO_ACCESS_KEY || 'minioadmin',
        secretKey: '***' + (process.env.MINIO_SECRET_KEY || 'minioadmin').slice(-4)
      },
      timestamp: new Date().toISOString()
    });
  }
});

// Обработка ошибок
app.use((err, req, res, next) => {
  console.error('Error:', err);
  res.status(500).json({
    error: 'Internal server error',
    message: err.message
  });
});

// Запуск сервера
connectDB().then(() => {
  app.listen(PORT, () => {
    console.log(`Star Gift Admin API running on http://localhost:${PORT}`);
  });
});

// Корректное завершение работы
process.on('SIGINT', async () => {
  await mongoClient.close();
  process.exit(0);
});

