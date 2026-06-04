import express from 'express';
import { ObjectId } from 'mongodb';

const router = express.Router();

// Формирует ID уведомления
function createNotificationId() {
  return `notification-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
}

// Возвращает текущую метку времени в формате Unix
function getCurrentTimestamp() {
  return Math.floor(Date.now() / 1000);
}

// GET /api/service-notifications - получить все шаблоны уведомлений
router.get('/', async (req, res) => {
  try {
    const collection = req.db.collection('ReadModel-ServiceNotificationTemplateReadModel');
    
    const templates = await collection.find({}).sort({ CreatedDate: -1 }).toArray();
    
    res.json({
      success: true,
      count: templates.length,
      templates: templates
    });
  } catch (error) {
    console.error('Error fetching notification templates:', error);
    res.status(500).json({
      success: false,
      error: error.message
    });
  }
});

// GET /api/service-notifications/:id - получить конкретный шаблон
router.get('/:id', async (req, res) => {
  try {
    const collection = req.db.collection('ReadModel-ServiceNotificationTemplateReadModel');
    
    const template = await collection.findOne({ Id: req.params.id });
    
    if (!template) {
      return res.status(404).json({
        success: false,
        error: 'Template not found'
      });
    }
    
    res.json({
      success: true,
      template: template
    });
  } catch (error) {
    console.error('Error fetching notification template:', error);
    res.status(500).json({
      success: false,
      error: error.message
    });
  }
});

// POST /api/service-notifications - создать новый шаблон
router.post('/', async (req, res) => {
  try {
    const { type, title, message, mediaUrl, mediaType, isPopup, isActive } = req.body;
    
    if (!type || !title || !message) {
      return res.status(400).json({
        success: false,
        error: 'Type, title, and message are required'
      });
    }
    
    const collection = req.db.collection('ReadModel-ServiceNotificationTemplateReadModel');
    
    const notificationId = createNotificationId();
    const currentDate = getCurrentTimestamp();
    
    const template = {
      _id: notificationId,
      Id: notificationId,
      Type: type,
      Title: title,
      Message: message,
      MediaUrl: mediaUrl || null,
      MediaType: mediaType || null,
      IsPopup: isPopup !== undefined ? isPopup : true,
      IsActive: isActive !== undefined ? isActive : true,
      CreatedDate: currentDate,
      UpdatedDate: null
    };
    
    await collection.insertOne(template);
    
    console.log(`Created notification template: ${notificationId} (${type})`);
    
    res.json({
      success: true,
      template: template
    });
  } catch (error) {
    console.error('Error creating notification template:', error);
    res.status(500).json({
      success: false,
      error: error.message
    });
  }
});

// PUT /api/service-notifications/:id - обновить шаблон
router.put('/:id', async (req, res) => {
  try {
    const { type, title, message, mediaUrl, mediaType, isPopup, isActive } = req.body;
    
    const collection = req.db.collection('ReadModel-ServiceNotificationTemplateReadModel');
    
    const updateFields = {
      UpdatedDate: getCurrentTimestamp()
    };
    
    if (type !== undefined) updateFields.Type = type;
    if (title !== undefined) updateFields.Title = title;
    if (message !== undefined) updateFields.Message = message;
    if (mediaUrl !== undefined) updateFields.MediaUrl = mediaUrl;
    if (mediaType !== undefined) updateFields.MediaType = mediaType;
    if (isPopup !== undefined) updateFields.IsPopup = isPopup;
    if (isActive !== undefined) updateFields.IsActive = isActive;
    
    const result = await collection.updateOne(
      { Id: req.params.id },
      { $set: updateFields }
    );
    
    if (result.matchedCount === 0) {
      return res.status(404).json({
        success: false,
        error: 'Template not found'
      });
    }
    
    const updatedTemplate = await collection.findOne({ Id: req.params.id });
    
    console.log(`Updated notification template: ${req.params.id}`);
    
    res.json({
      success: true,
      template: updatedTemplate
    });
  } catch (error) {
    console.error('Error updating notification template:', error);
    res.status(500).json({
      success: false,
      error: error.message
    });
  }
});

// DELETE /api/service-notifications/:id - удалить шаблон
router.delete('/:id', async (req, res) => {
  try {
    const collection = req.db.collection('ReadModel-ServiceNotificationTemplateReadModel');
    
    const result = await collection.deleteOne({ Id: req.params.id });
    
    if (result.deletedCount === 0) {
      return res.status(404).json({
        success: false,
        error: 'Template not found'
      });
    }
    
    console.log(`Deleted notification template: ${req.params.id}`);
    
    res.json({
      success: true,
      message: 'Template deleted successfully'
    });
  } catch (error) {
    console.error('Error deleting notification template:', error);
    res.status(500).json({
      success: false,
      error: error.message
    });
  }
});

// POST /api/service-notifications/:id/send - отправить уведомление пользователям
router.post('/:id/send', async (req, res) => {
  try {
    const { userIds, sendToAll } = req.body;

    console.log(`Database name: ${req.db.databaseName}`);
    console.log(`sendToAll: ${sendToAll}, userIds: ${userIds}`);

    // Загружаем шаблон
    const templatesCollection = req.db.collection('ReadModel-ServiceNotificationTemplateReadModel');
    const template = await templatesCollection.findOne({ Id: req.params.id });
    
    if (!template) {
      return res.status(404).json({
        success: false,
        error: 'Template not found'
      });
    }
    
    if (!template.IsActive) {
      return res.status(400).json({
        success: false,
        error: 'Template is not active'
      });
    }
    
    // Готовим данные уведомления
    const notification = {
      type: template.Type,
      message: template.Message,
      popup: template.IsPopup,
      mediaUrl: template.MediaUrl,
      mediaType: template.MediaType,
      timestamp: getCurrentTimestamp()
    };
    
    let targetUserIds = [];
    
    if (sendToAll) {
      // Берём все ID пользователей из eventflow-userreadmodel (в EventFlow имена в нижнем регистре)
      const usersCollection = req.db.collection('eventflow-userreadmodel');
      const users = await usersCollection.find({}).project({ UserId: 1 }).toArray();
      console.log(`Found ${users.length} users in database`);
      console.log(`First 3 users:`, users.slice(0, 3));
      targetUserIds = users.map(u => u.UserId).filter(id => id != null && id !== 777000 && id !== 569999);

      console.log(`Sending notification to ALL users (${targetUserIds.length} users, excluding system users)`);
    } else if (userIds && Array.isArray(userIds) && userIds.length > 0) {
      targetUserIds = userIds.map(id => parseInt(id));

      console.log(`Sending notification to ${targetUserIds.length} specific users`);
    } else {
      return res.status(400).json({
        success: false,
        error: 'Either sendToAll must be true or userIds must be provided'
      });
    }
    
    // Сохраняем события уведомлений в коллекцию для последующей обработки
    const notificationEventsCollection = req.db.collection('ServiceNotificationEvents');
    
    const events = targetUserIds.map(userId => ({
      userId: userId,
      templateId: template.Id,
      type: notification.type,
      message: notification.message,
      popup: notification.popup,
      mediaUrl: notification.mediaUrl,
      mediaType: notification.mediaType,
      createdAt: new Date(),
      processed: false
    }));
    
    if (events.length > 0) {
      await notificationEventsCollection.insertMany(events);
    }
    
    console.log(`Created ${events.length} notification events for processing`);

    // В реальной реализации здесь запускался бы C#-сервис.
    // Пока возвращаем успех, а события обработает фоновый воркер.

    res.json({
      success: true,
      message: `Notification queued for ${targetUserIds.length} users`,
      targetUserCount: targetUserIds.length,
      notification: notification
    });
    
  } catch (error) {
    console.error('Error sending notification:', error);
    res.status(500).json({
      success: false,
      error: error.message
    });
  }
});

// GET /api/service-notifications/events/pending - получить необработанные события уведомлений
router.get('/events/pending', async (req, res) => {
  try {
    const collection = db.collection('ServiceNotificationEvents');
    
    const pendingEvents = await collection
      .find({ processed: false })
      .sort({ createdAt: 1 })
      .limit(100)
      .toArray();
    
    res.json({
      success: true,
      count: pendingEvents.length,
      events: pendingEvents
    });
  } catch (error) {
    console.error('Error fetching pending events:', error);
    res.status(500).json({
      success: false,
      error: error.message
    });
  }
});

// POST /api/service-notifications/events/:id/mark-processed - пометить событие как обработанное
router.post('/events/:id/mark-processed', async (req, res) => {
  try {
    const collection = db.collection('ServiceNotificationEvents');
    
    const result = await collection.updateOne(
      { _id: new ObjectId(req.params.id) },
      { 
        $set: { 
          processed: true,
          processedAt: new Date()
        }
      }
    );
    
    if (result.matchedCount === 0) {
      return res.status(404).json({
        success: false,
        error: 'Event not found'
      });
    }
    
    res.json({
      success: true,
      message: 'Event marked as processed'
    });
  } catch (error) {
    console.error('Error marking event as processed:', error);
    res.status(500).json({
      success: false,
      error: error.message
    });
  }
});

export default router;
