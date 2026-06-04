import { Client } from 'minio';
import fs from 'fs/promises';
import path from 'path';

const BUCKET_NAME = 'tg-files';

// Ленивая инициализация: создаём клиента только при первом обращении
let minioClient = null;
let minioConfig = null;

function getMinioClient() {
  if (!minioClient) {
    // Конфигурация MinIO (должна совпадать с docker-compose.yml)
    minioConfig = {
      endPoint: process.env.MINIO_ENDPOINT || 'localhost',
      port: parseInt(process.env.MINIO_PORT || '9000'),
      useSSL: false,
      accessKey: process.env.MINIO_ACCESS_KEY || 'test',        // совпадает с .env Docker
      secretKey: process.env.MINIO_SECRET_KEY || '12345678'     // совпадает с .env Docker
    };

    // При первом использовании выводим конфигурацию (секретный ключ скрываем)
    console.log('MinIO Configuration:', {
      endPoint: minioConfig.endPoint,
      port: minioConfig.port,
      useSSL: minioConfig.useSSL,
      accessKey: minioConfig.accessKey,
      secretKey: '***' + minioConfig.secretKey.slice(-4) // показываем только последние 4 символа
    });

    minioClient = new Client(minioConfig);
  }
  return minioClient;
}

/**
 * Проверяет наличие bucket'а и создаёт его при отсутствии
 */
export async function ensureBucket() {
  const client = getMinioClient();
  try {
    console.log(`Checking if bucket "${BUCKET_NAME}" exists...`);
    const exists = await client.bucketExists(BUCKET_NAME);

    if (!exists) {
      console.log(`Bucket "${BUCKET_NAME}" does not exist, creating...`);
      await client.makeBucket(BUCKET_NAME, 'us-east-1');
      console.log(`Created MinIO bucket: ${BUCKET_NAME}`);
    } else {
      console.log(`Bucket "${BUCKET_NAME}" already exists`);
    }
  } catch (error) {
    console.error('Failed to ensure bucket:', {
      message: error.message,
      code: error.code,
      statusCode: error.statusCode,
      endpoint: minioConfig.endPoint + ':' + minioConfig.port,
      bucket: BUCKET_NAME
    });
    throw error;
  }
}

/**
 * Загружает файл в MinIO
 * @param {string} localFilePath - путь к локальному файлу
 * @param {number} documentId - ID документа, используется как имя объекта
 * @param {string} mimeType - MIME-тип файла
 * @returns {Promise<string>} имя объекта в MinIO
 */
export async function uploadFile(localFilePath, documentId, mimeType) {
  const client = getMinioClient();
  try {
    // Убеждаемся, что bucket существует
    await ensureBucket();

    // file-server ожидает файлы в корне без расширения.
    // Сохраняем под именем {documentId} (без папки и расширения) —
    // именно так их ищет file-server из MyTelegram
    const objectName = `${documentId}`;

    // Получаем сведения о файле
    const stats = await fs.stat(localFilePath);

    // Загружаем в MinIO
    const metaData = {
      'Content-Type': mimeType,
      'document-id': documentId.toString()
    };

    await client.fPutObject(
      BUCKET_NAME,
      objectName,
      localFilePath,
      metaData
    );

    console.log(`Uploaded to MinIO: ${objectName} (${stats.size} bytes)`);

    return objectName;
  } catch (error) {
    console.error('Failed to upload file to MinIO:', error);
    throw error;
  }
}

/**
 * Удаляет файл из MinIO
 * @param {number} documentId - ID документа
 */
export async function deleteFile(documentId) {
  const client = getMinioClient();
  try {
    // Удаляем из корня без расширения
    const objectName = `${documentId}`;

    try {
      await client.removeObject(BUCKET_NAME, objectName);
      console.log(`Deleted from MinIO: ${objectName}`);
    } catch (err) {
      console.warn(`File not found in MinIO: ${objectName}`);
    }
  } catch (error) {
    console.error('Failed to delete file from MinIO:', error);
    // Не пробрасываем ошибку — файла может не быть
  }
}

/**
 * Проверяет, существует ли файл в MinIO
 * @param {number} documentId - ID документа
 * @returns {Promise<boolean>}
 */
export async function fileExists(documentId) {
  const client = getMinioClient();
  try {
    // Проверяем в корне без расширения
    const objectName = `${documentId}`;

    try {
      await client.statObject(BUCKET_NAME, objectName);
      return true;
    } catch (err) {
      return false;
    }
  } catch (error) {
    return false;
  }
}

export default {
  uploadFile,
  deleteFile,
  fileExists,
  ensureBucket
};
