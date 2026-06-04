import express from 'express';
import { MongoClient } from 'mongodb';

const router = express.Router();

const MONGO_URI = process.env.MONGO_URI || 'mongodb://localhost:27017';
const DB_NAME = 'tg';

// Get MongoDB client
async function getMongoClient() {
  const client = new MongoClient(MONGO_URI);
  await client.connect();
  return client;
}

// Get all frozen accounts
router.get('/', async (req, res) => {
  let client;
  try {
    client = await getMongoClient();
    const db = client.db(DB_NAME);
    const frozenCollection = db.collection('ReadModel-UserFrozenAccountReadModel');
    
    const frozenAccounts = await frozenCollection.find({}).sort({ CreatedDate: -1 }).toArray();
    
    res.json({
      success: true,
      data: frozenAccounts,
      count: frozenAccounts.length
    });
  } catch (error) {
    console.error('Error fetching frozen accounts:', error);
    res.status(500).json({ success: false, error: error.message });
  } finally {
    if (client) await client.close();
  }
});

// Get frozen account by user ID
router.get('/user/:userId', async (req, res) => {
  let client;
  try {
    const userId = parseInt(req.params.userId);
    
    client = await getMongoClient();
    const db = client.db(DB_NAME);
    const frozenCollection = db.collection('ReadModel-UserFrozenAccountReadModel');
    
    const frozenAccount = await frozenCollection.findOne({ UserId: userId });
    
    if (!frozenAccount) {
      return res.status(404).json({ success: false, error: 'Frozen account not found' });
    }
    
    res.json({
      success: true,
      data: frozenAccount
    });
  } catch (error) {
    console.error('Error fetching frozen account:', error);
    res.status(500).json({ success: false, error: error.message });
  } finally {
    if (client) await client.close();
  }
});

// Freeze account
router.post('/freeze', async (req, res) => {
  let client;
  try {
    const {
      userId: userIdRaw,
      reason,
      durationDays = 7,
      moderatorUserId,
      note
    } = req.body;
    
    if (!userIdRaw) {
      return res.status(400).json({ success: false, error: 'userId is required' });
    }
    
    // Преобразуем userId в число
    const userId = parseInt(userIdRaw);
    if (isNaN(userId)) {
      return res.status(400).json({ success: false, error: 'userId must be a valid number' });
    }
    
    client = await getMongoClient();
    const db = client.db(DB_NAME);
    
    const usersCollection = db.collection('eventflow-userreadmodel');
    const frozenCollection = db.collection('ReadModel-UserFrozenAccountReadModel');
    
    // Check if user exists
    let user = await usersCollection.findOne({ UserId: userId });
    if (!user) {
      // Try alternative collection
      user = await db.collection('ReadModel-UserReadModel').findOne({ UserId: userId });
      if (!user) {
        return res.status(404).json({ 
          success: false, 
          error: `User ${userId} not found. Make sure the user is registered in the system.`,
          hint: 'The user must have logged in at least once to MyTelegram'
        });
      }
    }
    
    // Check if already frozen (any status)
    const existingFrozen = await frozenCollection.findOne({ UserId: userId });
    
    if (existingFrozen) {
      // Если уже заморожен с Status: 1 (Active)
      if (existingFrozen.Status === 1) {
        return res.status(400).json({ 
          success: false, 
          error: 'User is already frozen with active status' 
        });
      }
      
      // Если запись существует но не активна (Status: 2 или 3), обновляем её
      const now = Math.floor(Date.now() / 1000);
      const freezeUntil = now + (durationDays * 24 * 60 * 60);
      
      await frozenCollection.updateOne(
        { UserId: userId },
        {
          $set: {
            FreezeSinceDate: now,
            FreezeUntilDate: freezeUntil,
            Reason: reason || 4,
            Status: 1, // Active
            AppealUrl: 'https://t.me/spambot',
            ModeratorUserId: moderatorUserId || null,
            FreezeNote: note || null,
            LastModifiedDate: new Date(),
            Metadata: {
              frozenBy: 'admin_panel',
              timestamp: new Date().toISOString()
            }
          }
        }
      );
      
      // Update user record
      await usersCollection.updateOne(
        { UserId: userId },
        {
          $set: {
            IsFrozen: true,
            FreezeSinceDate: now,
            FreezeUntilDate: freezeUntil,
            FreezeReason: reason || 4,
            FreezeAppealUrl: 'https://t.me/spambot'
          }
        }
      );
      
      return res.json({
        success: true,
        message: 'Account re-frozen successfully (updated existing record)',
        data: { ...existingFrozen, Status: 1, FreezeSinceDate: now, FreezeUntilDate: freezeUntil }
      });
    }
    
    // Новая заморозка - создаем запись
    const now = Math.floor(Date.now() / 1000);
    const freezeUntil = now + (durationDays * 24 * 60 * 60);
    
    const frozenDoc = {
      _id: `frozen-${userId}`,
      UserId: userId,
      FreezeSinceDate: now,
      FreezeUntilDate: freezeUntil,
      Reason: reason || 4, // Default: TosViolation
      Status: 1, // Active
      AppealUrl: 'https://t.me/spambot',
      ModeratorUserId: moderatorUserId || null,
      FreezeNote: note || null,
      CreatedDate: new Date(),
      Metadata: {
        frozenBy: 'admin_panel',
        timestamp: new Date().toISOString()
      }
    };
    
    await frozenCollection.insertOne(frozenDoc);
    
    // Update user record
    await usersCollection.updateOne(
      { UserId: userId },
      {
        $set: {
          IsFrozen: true,
          FreezeSinceDate: now,
          FreezeUntilDate: freezeUntil,
          FreezeReason: reason || 4,
          FreezeAppealUrl: 'https://t.me/spambot'
        }
      }
    );
    
    res.json({
      success: true,
      message: 'Account frozen successfully',
      data: frozenDoc
    });
    
  } catch (error) {
    console.error('Error freezing account:', error);
    res.status(500).json({ success: false, error: error.message });
  } finally {
    if (client) await client.close();
  }
});

// Unfreeze account
router.post('/unfreeze', async (req, res) => {
  let client;
  try {
    const { userId: userIdRaw, moderatorUserId, note } = req.body;
    
    if (!userIdRaw) {
      return res.status(400).json({ success: false, error: 'userId is required' });
    }
    
    const userId = parseInt(userIdRaw);
    
    client = await getMongoClient();
    const db = client.db(DB_NAME);
    
    const usersCollection = db.collection('eventflow-userreadmodel');
    const frozenCollection = db.collection('ReadModel-UserFrozenAccountReadModel');
    
    // Check if frozen
    const frozenAccount = await frozenCollection.findOne({ UserId: userId });
    if (!frozenAccount) {
      return res.status(404).json({ success: false, error: 'Frozen account not found' });
    }
    
    // Update frozen record
    await frozenCollection.updateOne(
      { UserId: userId },
      {
        $set: {
          Status: 3, // Approved
          LastModifiedDate: new Date()
        }
      }
    );
    
    // Update user record
    await usersCollection.updateOne(
      { UserId: userId },
      {
        $set: {
          IsFrozen: false
        },
        $unset: {
          FreezeSinceDate: "",
          FreezeUntilDate: "",
          FreezeReason: "",
          FreezeAppealUrl: ""
        }
      }
    );
    
    res.json({
      success: true,
      message: 'Account unfrozen successfully'
    });
    
  } catch (error) {
    console.error('Error unfreezing account:', error);
    res.status(500).json({ success: false, error: error.message });
  } finally {
    if (client) await client.close();
  }
});

// Get appeals
router.get('/appeals', async (req, res) => {
  let client;
  try {
    client = await getMongoClient();
    const db = client.db(DB_NAME);
    const appealsCollection = db.collection('ReadModel-UserFrozenAppealHistoryReadModel');
    
    const appeals = await appealsCollection.find({}).sort({ SubmittedDate: -1 }).toArray();
    
    res.json({
      success: true,
      data: appeals,
      count: appeals.length
    });
  } catch (error) {
    console.error('Error fetching appeals:', error);
    res.status(500).json({ success: false, error: error.message });
  } finally {
    if (client) await client.close();
  }
});

// Review appeal
router.post('/appeals/:appealId/review', async (req, res) => {
  let client;
  try {
    const { appealId } = req.params;
    const { status, moderatorUserId, reviewNote } = req.body;
    
    if (!status || ![2, 3].includes(status)) {
      return res.status(400).json({ success: false, error: 'Invalid status (2=Approved, 3=Rejected)' });
    }
    
    client = await getMongoClient();
    const db = client.db(DB_NAME);
    const appealsCollection = db.collection('ReadModel-UserFrozenAppealHistoryReadModel');
    
    const appeal = await appealsCollection.findOne({ AppealId: appealId });
    if (!appeal) {
      return res.status(404).json({ success: false, error: 'Appeal not found' });
    }
    
    // Update appeal
    await appealsCollection.updateOne(
      { AppealId: appealId },
      {
        $set: {
          Status: status,
          ReviewerModeratorId: moderatorUserId,
          ReviewNote: reviewNote,
          ReviewedDate: new Date()
        }
      }
    );
    
    // If approved, unfreeze the account
    if (status === 2) {
      const usersCollection = db.collection('eventflow-userreadmodel');
      const frozenCollection = db.collection('ReadModel-UserFrozenAccountReadModel');
      
      await frozenCollection.updateOne(
        { UserId: appeal.UserId },
        { $set: { Status: 3 } } // Approved
      );
      
      await usersCollection.updateOne(
        { UserId: appeal.UserId },
        {
          $set: { IsFrozen: false },
          $unset: {
            FreezeSinceDate: "",
            FreezeUntilDate: "",
            FreezeReason: "",
            FreezeAppealUrl: ""
          }
        }
      );
    }
    
    res.json({
      success: true,
      message: status === 2 ? 'Appeal approved' : 'Appeal rejected'
    });
    
  } catch (error) {
    console.error('Error reviewing appeal:', error);
    res.status(500).json({ success: false, error: error.message });
  } finally {
    if (client) await client.close();
  }
});

export default router;
