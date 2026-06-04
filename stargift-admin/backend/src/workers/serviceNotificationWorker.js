import { MongoClient } from 'mongodb';
import dotenv from 'dotenv';
import { fileURLToPath } from 'url';
import { dirname, join } from 'path';

// Определяем текущий каталог (в ES-модулях нет __dirname по умолчанию)
const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

// Загружаем переменные окружения
const envPath = join(__dirname, '..', '..', '.env');
dotenv.config({ path: envPath });

const mongoUrl = process.env.MONGODB_URI || 'mongodb://localhost:27017';
const dbName = process.env.DB_NAME || 'tg';
const adminApiUrl = process.env.ADMIN_API_URL || 'http://localhost:5555';

let db;
let isProcessing = false;

// Подключаемся к MongoDB
async function connectDB() {
  try {
    const client = await MongoClient.connect(mongoUrl);
    db = client.db(dbName);
    console.log('Service Notification Worker connected to MongoDB');
    return client;
  } catch (error) {
    console.error('MongoDB connection error:', error);
    process.exit(1);
  }
}

// Отправляем уведомление через HTTP API
async function sendNotificationViaApi(event) {
  const payload = {
    userId: event.userId,
    type: event.type,
    message: event.message,
    popup: event.popup,
    mediaUrl: event.mediaUrl,
    mediaType: event.mediaType
  };
  
  console.log(`  Calling Admin API: ${adminApiUrl}/api/service-notifications/send`);
  console.log(`  Payload:`, JSON.stringify(payload, null, 2));

  const response = await fetch(`${adminApiUrl}/api/service-notifications/send`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });

  console.log(`  Response status: ${response.status}`);

  if (!response.ok) {
    const errorText = await response.text();
    console.error(`  API Error: ${errorText}`);
    throw new Error(`HTTP ${response.status}: ${errorText}`);
  }

  const result = await response.json();
  console.log(`  API Response:`, result);

  return result;
}

// Обрабатываем необработанные события уведомлений
async function processPendingNotifications() {
  if (isProcessing) {
    return; // обработка уже идёт
  }

  try {
    isProcessing = true;
    
    const eventsCollection = db.collection('ServiceNotificationEvents');

    // Берём необработанные события (не более 10 за раз)
    const pendingEvents = await eventsCollection
      .find({ processed: false })
      .sort({ createdAt: 1 })
      .limit(10)
      .toArray();

    if (pendingEvents.length === 0) {
      return; // необработанных событий нет
    }

    console.log(`\nProcessing ${pendingEvents.length} pending notifications...`);

    for (const event of pendingEvents) {
      try {
        console.log(`  Sending notification to user ${event.userId} (type: ${event.type})`);

        // Отправляем через HTTP API
        await sendNotificationViaApi(event);

        // Помечаем как обработанное
        await eventsCollection.updateOne(
          { _id: event._id },
          { 
            $set: { 
              processed: true,
              processedAt: new Date(),
              status: 'sent'
            }
          }
        );

        console.log(`  Notification sent to user ${event.userId}`);
      } catch (error) {
        console.error(`  Failed to send notification to user ${event.userId}:`, error.message);

        // Помечаем как неудачное
        await eventsCollection.updateOne(
          { _id: event._id },
          { 
            $set: { 
              processed: true,
              processedAt: new Date(),
              status: 'failed',
              error: error.message
            }
          }
        );
      }
    }

    console.log(`Processed ${pendingEvents.length} notifications\n`);
  } catch (error) {
    console.error('Error processing notifications:', error);
  } finally {
    isProcessing = false;
  }
}

// Главный цикл воркера
async function startWorker() {
  console.log('Service Notification Worker started');
  console.log(`Using Admin API: ${adminApiUrl}`);
  console.log('Checking for pending notifications every 5 seconds...\n');

  await connectDB();

  // Сразу обрабатываем события при запуске
  await processPendingNotifications();

  // Затем проверяем каждые 5 секунд
  setInterval(async () => {
    await processPendingNotifications();
  }, 5000); // проверка каждые 5 секунд
}

// Корректное завершение работы
process.on('SIGINT', () => {
  console.log('\nShutting down worker...');
  process.exit(0);
});

process.on('SIGTERM', () => {
  console.log('\nShutting down worker...');
  process.exit(0);
});

// Запускаем воркер
startWorker().catch(console.error);
