import express from 'express';
const router = express.Router();

// Список всех пользователей с постраничной разбивкой
router.get('/', async (req, res) => {
  try {
    const { page = 1, limit = 20, search = '' } = req.query;
    const skip = (parseInt(page) - 1) * parseInt(limit);
    
    const filter = {};
    if (search) {
      filter.$or = [
        { FirstName: { $regex: search, $options: 'i' } },
        { LastName: { $regex: search, $options: 'i' } },
        { UserName: { $regex: search, $options: 'i' } },
        { Phone: { $regex: search, $options: 'i' } }
      ];
    }
    
    const [users, total] = await Promise.all([
      req.db.collection('eventflow-userreadmodel')
        .find(filter)
        .sort({ UserId: -1 })
        .skip(skip)
        .limit(parseInt(limit))
        .toArray(),
      req.db.collection('eventflow-userreadmodel').countDocuments(filter)
    ]);
    
    res.json({
      success: true,
      users,
      pagination: {
        page: parseInt(page),
        limit: parseInt(limit),
        total,
        pages: Math.ceil(total / parseInt(limit))
      }
    });
  } catch (error) {
    console.error('Get users error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Поиск пользователей по телефону или имени пользователя
router.get('/search', async (req, res) => {
  try {
    const { phone, username } = req.query;

    if (!phone && !username) {
      return res.status(400).json({ error: 'phone or username parameter required' });
    }

    let query = {};

    if (phone) {
      // Поиск по номеру телефона
      query.PhoneNumber = phone;
    } else if (username) {
      // Поиск по имени пользователя (без учёта регистра)
      query.UserName = new RegExp(`^${username}$`, 'i');
    }
    
    const users = await req.db
      .collection('eventflow-userreadmodel')
      .find(query)
      .limit(10)
      .toArray();
    
    res.json(users);
  } catch (error) {
    console.error('User search error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Получить пользователя по ID
router.get('/:userId', async (req, res) => {
  try {
    const userId = parseInt(req.params.userId);
    
    const user = await req.db
      .collection('eventflow-userreadmodel')
      .findOne({ UserId: userId });
    
    if (!user) {
      return res.status(404).json({ error: 'User not found' });
    }
    
    res.json(user);
  } catch (error) {
    console.error('Get user error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Заморозить (ограничить) аккаунт пользователя
router.post('/:userId/freeze', async (req, res) => {
  try {
    const userId = parseInt(req.params.userId);
    const { reason = 'Account restricted for violating Terms of Service' } = req.body;
    
    const user = await req.db
      .collection('eventflow-userreadmodel')
      .findOne({ UserId: userId });
    
    if (!user) {
      return res.status(404).json({ error: 'User not found' });
    }
    
    // Переводим пользователя в замороженное (ограниченное) состояние
    await req.db
      .collection('eventflow-userreadmodel')
      .updateOne(
        { UserId: userId },
        { 
          $set: { 
            Restricted: true,
            RestrictionReason: reason
          }
        }
      );
    
    res.json({
      success: true,
      message: `User ${userId} has been frozen`,
      userId,
      reason
    });
  } catch (error) {
    console.error('Freeze user error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Разморозить (снять ограничение) аккаунт пользователя
router.post('/:userId/unfreeze', async (req, res) => {
  try {
    const userId = parseInt(req.params.userId);
    
    const user = await req.db
      .collection('eventflow-userreadmodel')
      .findOne({ UserId: userId });
    
    if (!user) {
      return res.status(404).json({ error: 'User not found' });
    }
    
    // Снимаем ограничение
    await req.db
      .collection('eventflow-userreadmodel')
      .updateOne(
        { UserId: userId },
        { 
          $set: { 
            Restricted: false,
            RestrictionReason: null
          }
        }
      );
    
    res.json({
      success: true,
      message: `User ${userId} has been unfrozen`,
      userId
    });
  } catch (error) {
    console.error('Unfreeze user error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Удалить аккаунт пользователя (пометить как удалённый)
router.delete('/:userId', async (req, res) => {
  try {
    const userId = parseInt(req.params.userId);
    
    const user = await req.db
      .collection('eventflow-userreadmodel')
      .findOne({ UserId: userId });
    
    if (!user) {
      return res.status(404).json({ error: 'User not found' });
    }
    
    // Помечаем пользователя как удалённого
    await req.db
      .collection('eventflow-userreadmodel')
      .updateOne(
        { UserId: userId },
        { 
          $set: { 
            IsDeleted: true,
            Restricted: true,
            RestrictionReason: 'Account deleted by administrator'
          }
        }
      );
    
    res.json({
      success: true,
      message: `User ${userId} has been deleted`,
      userId
    });
  } catch (error) {
    console.error('Delete user error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Задать анимацию заморозки для пользователя
router.post('/:userId/frozen-animation', async (req, res) => {
  try {
    const userId = parseInt(req.params.userId);
    const { documentId } = req.body;
    
    if (!documentId) {
      return res.status(400).json({ error: 'Document ID is required' });
    }
    
    const user = await req.db
      .collection('eventflow-userreadmodel')
      .findOne({ UserId: userId });
    
    if (!user) {
      return res.status(404).json({ error: 'User not found' });
    }
    
    // Сохраняем ID документа анимации заморозки
    await req.db
      .collection('eventflow-userreadmodel')
      .updateOne(
        { UserId: userId },
        { 
          $set: { 
            FrozenAnimationDocumentId: parseInt(documentId)
          }
        }
      );
    
    res.json({
      success: true,
      message: `Frozen animation set for user ${userId}`,
      userId,
      documentId: parseInt(documentId)
    });
  } catch (error) {
    console.error('Set frozen animation error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Убрать анимацию заморозки у пользователя
router.delete('/:userId/frozen-animation', async (req, res) => {
  try {
    const userId = parseInt(req.params.userId);
    
    const user = await req.db
      .collection('eventflow-userreadmodel')
      .findOne({ UserId: userId });
    
    if (!user) {
      return res.status(404).json({ error: 'User not found' });
    }
    
    // Убираем анимацию заморозки
    await req.db
      .collection('eventflow-userreadmodel')
      .updateOne(
        { UserId: userId },
        { 
          $unset: { 
            FrozenAnimationDocumentId: ""
          }
        }
      );
    
    res.json({
      success: true,
      message: `Frozen animation removed from user ${userId}`,
      userId
    });
  } catch (error) {
    console.error('Remove frozen animation error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Выдать пользователю premium
router.post('/:userId/premium', async (req, res) => {
  try {
    const userId = parseInt(req.params.userId);
    
    const user = await req.db
      .collection('eventflow-userreadmodel')
      .findOne({ UserId: userId });
    
    if (!user) {
      return res.status(404).json({ error: 'User not found' });
    }
    
    // Выдаём статус premium
    await req.db
      .collection('eventflow-userreadmodel')
      .updateOne(
        { UserId: userId },
        { 
          $set: { 
            Premium: true,
            PremiumGifts: true
          }
        }
      );
    
    res.json({
      success: true,
      message: `Premium given to user ${userId}`,
      userId
    });
  } catch (error) {
    console.error('Give premium error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Снять premium с пользователя
router.delete('/:userId/premium', async (req, res) => {
  try {
    const userId = parseInt(req.params.userId);
    
    const user = await req.db
      .collection('eventflow-userreadmodel')
      .findOne({ UserId: userId });
    
    if (!user) {
      return res.status(404).json({ error: 'User not found' });
    }
    
    // Снимаем статус premium
    await req.db
      .collection('eventflow-userreadmodel')
      .updateOne(
        { UserId: userId },
        { 
          $set: { 
            Premium: false,
            PremiumGifts: false
          }
        }
      );
    
    res.json({
      success: true,
      message: `Premium removed from user ${userId}`,
      userId
    });
  } catch (error) {
    console.error('Remove premium error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Получить настройки иконки заморозки
router.get('/settings/frozen-icon', async (req, res) => {
  try {
    const settings = await req.db
      .collection('admin-settings')
      .findOne({ key: 'frozen_icon' });
    
    res.json({
      type: settings?.type || 'emoji', // значение 'emoji' или 'animation'
      emoji: settings?.emoji || '❄️',
      animationUrl: settings?.animationUrl || null
    });
  } catch (error) {
    console.error('Get frozen icon error:', error);
    res.status(500).json({ error: error.message });
  }
});

// Обновить настройки иконки заморозки
router.post('/settings/frozen-icon', async (req, res) => {
  try {
    const { type, emoji, animationUrl } = req.body;
    
    await req.db
      .collection('admin-settings')
      .updateOne(
        { key: 'frozen_icon' },
        { 
          $set: { 
            key: 'frozen_icon',
            type: type || 'emoji',
            emoji: emoji || '❄️',
            animationUrl: animationUrl || null,
            updatedAt: new Date()
          }
        },
        { upsert: true }
      );
    
    res.json({
      success: true,
      message: 'Frozen icon settings updated'
    });
  } catch (error) {
    console.error('Update frozen icon error:', error);
    res.status(500).json({ error: error.message });
  }
});

export default router;
