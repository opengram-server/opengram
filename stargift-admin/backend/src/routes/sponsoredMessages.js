import express from 'express';
import Joi from 'joi';

const router = express.Router();

// Схема валидации
const sponsoredMessageSchema = Joi.object({
  channelId: Joi.number().integer().positive().required(),
  title: Joi.string().max(100).required(),
  message: Joi.string().max(500).required(),
  url: Joi.string().uri().required(),
  buttonText: Joi.string().max(50).required(),
  photoUrl: Joi.string().uri().optional().allow(null, ''),
  sponsorInfo: Joi.string().max(200).optional().allow(null, ''),
  additionalInfo: Joi.string().max(200).optional().allow(null, ''),
  isActive: Joi.boolean().default(true),
  recommended: Joi.boolean().default(false),
  canReport: Joi.boolean().default(true),
  postsBetween: Joi.number().integer().min(1).max(100).default(10),
  expiresDate: Joi.number().integer().optional().allow(null)
});

// Список всех рекламных сообщений
router.get('/', async (req, res) => {
  try {
    const { channelId, isActive } = req.query;
    
    const filter = {};
    if (channelId) filter.ChannelId = parseInt(channelId);
    if (isActive !== undefined) filter.IsActive = isActive === 'true';
    
    const messages = await req.db
      .collection('ReadModel-SponsoredMessageReadModel')
      .find(filter)
      .sort({ CreatedDate: -1 })
      .toArray();
    
    res.json({
      success: true,
      count: messages.length,
      messages
    });
  } catch (error) {
    console.error('Error fetching sponsored messages:', error);
    res.status(500).json({ 
      success: false, 
      error: error.message 
    });
  }
});

// Получить рекламное сообщение по ID
router.get('/:id', async (req, res) => {
  try {
    const message = await req.db
      .collection('ReadModel-SponsoredMessageReadModel')
      .findOne({ Id: req.params.id });
    
    if (!message) {
      return res.status(404).json({ 
        success: false, 
        error: 'Sponsored message not found' 
      });
    }
    
    res.json({
      success: true,
      message
    });
  } catch (error) {
    console.error('Error fetching sponsored message:', error);
    res.status(500).json({ 
      success: false, 
      error: error.message 
    });
  }
});

// Создать новое рекламное сообщение
router.post('/', async (req, res) => {
  try {
    const { error, value } = sponsoredMessageSchema.validate(req.body);
    if (error) {
      return res.status(400).json({ 
        success: false, 
        error: error.details[0].message 
      });
    }

    const now = Math.floor(Date.now() / 1000);
    const id = `sponsored-${value.channelId}-${now}`;

    const sponsoredMessage = {
      Id: id,
      ChannelId: value.channelId,
      Title: value.title,
      Message: value.message,
      Url: value.url,
      ButtonText: value.buttonText,
      PhotoUrl: value.photoUrl || null,
      SponsorInfo: value.sponsorInfo || null,
      AdditionalInfo: value.additionalInfo || null,
      IsActive: value.isActive,
      Recommended: value.recommended,
      CanReport: value.canReport,
      PostsBetween: value.postsBetween,
      CreatedDate: now,
      ExpiresDate: value.expiresDate || null,
      DisplayCount: 0,
      ClickCount: 0,
      Version: 1
    };

    await req.db
      .collection('ReadModel-SponsoredMessageReadModel')
      .insertOne(sponsoredMessage);

    console.log(`Created sponsored message: ${id} for channel ${value.channelId}`);
    
    res.status(201).json({
      success: true,
      message: sponsoredMessage
    });
  } catch (error) {
    console.error('Error creating sponsored message:', error);
    res.status(500).json({ 
      success: false, 
      error: error.message 
    });
  }
});

// Обновить рекламное сообщение
router.put('/:id', async (req, res) => {
  try {
    const { error, value } = sponsoredMessageSchema.validate(req.body);
    if (error) {
      return res.status(400).json({ 
        success: false, 
        error: error.details[0].message 
      });
    }

    const updateData = {
      ChannelId: value.channelId,
      Title: value.title,
      Message: value.message,
      Url: value.url,
      ButtonText: value.buttonText,
      PhotoUrl: value.photoUrl || null,
      SponsorInfo: value.sponsorInfo || null,
      AdditionalInfo: value.additionalInfo || null,
      IsActive: value.isActive,
      Recommended: value.recommended,
      CanReport: value.canReport,
      PostsBetween: value.postsBetween,
      ExpiresDate: value.expiresDate || null
    };

    const result = await req.db
      .collection('ReadModel-SponsoredMessageReadModel')
      .updateOne(
        { Id: req.params.id },
        { $set: updateData, $inc: { Version: 1 } }
      );

    if (result.matchedCount === 0) {
      return res.status(404).json({ 
        success: false, 
        error: 'Sponsored message not found' 
      });
    }

    console.log(`Updated sponsored message: ${req.params.id}`);
    
    res.json({
      success: true,
      message: 'Sponsored message updated successfully'
    });
  } catch (error) {
    console.error('Error updating sponsored message:', error);
    res.status(500).json({ 
      success: false, 
      error: error.message 
    });
  }
});

// Переключить статус активности
router.patch('/:id/toggle', async (req, res) => {
  try {
    const message = await req.db
      .collection('ReadModel-SponsoredMessageReadModel')
      .findOne({ Id: req.params.id });

    if (!message) {
      return res.status(404).json({ 
        success: false, 
        error: 'Sponsored message not found' 
      });
    }

    const newStatus = !message.IsActive;

    await req.db
      .collection('ReadModel-SponsoredMessageReadModel')
      .updateOne(
        { Id: req.params.id },
        { $set: { IsActive: newStatus }, $inc: { Version: 1 } }
      );

    console.log(`Toggled sponsored message ${req.params.id} to ${newStatus ? 'active' : 'inactive'}`);
    
    res.json({
      success: true,
      isActive: newStatus
    });
  } catch (error) {
    console.error('Error toggling sponsored message:', error);
    res.status(500).json({ 
      success: false, 
      error: error.message 
    });
  }
});

// Удалить рекламное сообщение
router.delete('/:id', async (req, res) => {
  try {
    const result = await req.db
      .collection('ReadModel-SponsoredMessageReadModel')
      .deleteOne({ Id: req.params.id });

    if (result.deletedCount === 0) {
      return res.status(404).json({ 
        success: false, 
        error: 'Sponsored message not found' 
      });
    }

    console.log(`Deleted sponsored message: ${req.params.id}`);
    
    res.json({
      success: true,
      message: 'Sponsored message deleted successfully'
    });
  } catch (error) {
    console.error('Error deleting sponsored message:', error);
    res.status(500).json({ 
      success: false, 
      error: error.message 
    });
  }
});

// Статистика по каналу
router.get('/stats/:channelId', async (req, res) => {
  try {
    const channelId = parseInt(req.params.channelId);
    
    const messages = await req.db
      .collection('ReadModel-SponsoredMessageReadModel')
      .find({ ChannelId: channelId })
      .toArray();

    const stats = {
      totalMessages: messages.length,
      activeMessages: messages.filter(m => m.IsActive).length,
      totalDisplays: messages.reduce((sum, m) => sum + (m.DisplayCount || 0), 0),
      totalClicks: messages.reduce((sum, m) => sum + (m.ClickCount || 0), 0),
      clickThroughRate: 0
    };

    if (stats.totalDisplays > 0) {
      stats.clickThroughRate = ((stats.totalClicks / stats.totalDisplays) * 100).toFixed(2);
    }

    res.json({
      success: true,
      channelId,
      stats
    });
  } catch (error) {
    console.error('Error fetching stats:', error);
    res.status(500).json({ 
      success: false, 
      error: error.message 
    });
  }
});

// Учёт перехода по ссылке (вызывается из ссылки отслеживания)
router.post('/click/:id', async (req, res) => {
  try {
    const { id } = req.params;
    
    const result = await req.db
      .collection('ReadModel-SponsoredMessageReadModel')
      .updateOne(
        { Id: id },
        { $inc: { ClickCount: 1 } }
      );

    if (result.matchedCount === 0) {
      return res.status(404).json({
        success: false,
        error: 'Sponsored message not found'
      });
    }

    console.log(`Click tracked for sponsored message: ${id}`);

    res.json({
      success: true,
      message: 'Click tracked successfully'
    });
  } catch (error) {
    console.error('Error tracking click:', error);
    res.status(500).json({
      success: false,
      error: error.message
    });
  }
});

export default router;
