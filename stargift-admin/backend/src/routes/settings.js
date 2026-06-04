import express from 'express';

const router = express.Router();

// Получить настройки иконки заморозки
router.get('/frozen-icon', async (req, res) => {
  try {
    // Берём глобальные настройки иконки заморозки из БД
    const settings = await req.db
      .collection('global_settings')
      .findOne({ key: 'frozen_icon' });

    if (!settings) {
      // Настройки по умолчанию
      return res.json({
        type: 'emoji',
        emoji: '❄️',
        documentId: null
      });
    }
    
    res.json(settings.value);
  } catch (error) {
    console.error('Get frozen icon settings error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Обновить настройки иконки заморозки
router.post('/frozen-icon', async (req, res) => {
  try {
    const { type, emoji, documentId } = req.body;
    
    const settings = {
      type: type || 'emoji',
      emoji: emoji || '❄️',
      documentId: documentId || null
    };
    
    // Сохраняем глобальные настройки
    await req.db
      .collection('global_settings')
      .updateOne(
        { key: 'frozen_icon' },
        { 
          $set: { 
            key: 'frozen_icon',
            value: settings,
            updatedAt: new Date()
          }
        },
        { upsert: true }
      );
    
    // Если выбрана анимация, применяем её ко всем замороженным пользователям
    if (type === 'animation' && documentId) {
      await req.db
        .collection('eventflow-userreadmodel')
        .updateMany(
          { Restricted: true },
          { 
            $set: { 
              FrozenAnimationDocumentId: parseInt(documentId)
            }
          }
        );
      
      console.log(`Applied frozen animation ${documentId} to all restricted users`);
    } else if (type === 'emoji') {
      // При переключении на эмодзи убираем анимацию у всех пользователей
      await req.db
        .collection('eventflow-userreadmodel')
        .updateMany(
          { Restricted: true },
          { 
            $unset: { 
              FrozenAnimationDocumentId: ""
            }
          }
        );
      
      console.log(`Removed frozen animation from all restricted users`);
    }
    
    res.json({
      success: true,
      message: 'Frozen icon settings updated',
      settings
    });
  } catch (error) {
    console.error('Update frozen icon settings error:', error);
    res.status(500).json({ error: error.message });
  }
});

export default router;
