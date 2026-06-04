import express from 'express';
import { ObjectId, Long } from 'mongodb';
import { v5 as uuidv5, v4 as uuidv4 } from 'uuid';

const router = express.Router();

// Пространство имён EventFlow (получено через рефлексию из EventFlow.Core.GuidFactories)
const EVENTFLOW_COMMANDS_NAMESPACE = '4286d89f-7f92-430b-8e00-e468fe3c3f59';

// POST /api/stars/issue - начислить звёзды пользователю
router.post('/issue', async (req, res) => {
    try {
        const { userId, amount, reason } = req.body;

        // Проверка входных данных
        if (!userId || !amount || amount <= 0) {
            return res.status(400).json({
                success: false,
                error: 'Invalid request. userId and positive amount are required.'
            });
        }

        const db = req.db;
        const eventsCollection = db.collection('eventflow.events');
        const starsCollection = db.collection('eventflow-starsreadmodel');
        const userCollection = db.collection('eventflow-userreadmodel');

        // Проверяем, существует ли пользователь
        const user = await userCollection.findOne({ UserId: parseInt(userId) });

        if (!user) {
            return res.status(404).json({
                success: false,
                error: 'User not found'
            });
        }

        // Генерируем детерминированный ID на основе пространства имён EventFlow
        const guid = uuidv5(`stars_${userId}`, EVENTFLOW_COMMANDS_NAMESPACE);
        const aggregateId = `stars-${guid}`;
        const documentId = aggregateId;

        console.log(`Generated StarsAggregate ID: ${aggregateId} for user ${userId}`);

        // --- Обновление write-модели EventFlow ---

        // Находим последнее событие агрегата, чтобы определить номер в последовательности
        const lastEvent = await eventsCollection.findOne(
            { AggregateId: aggregateId },
            { sort: { AggregateSequenceNumber: -1 }, projection: { AggregateSequenceNumber: 1, Data: 1 } }
        );

        let currentSequence = lastEvent ? lastEvent.AggregateSequenceNumber : 0;
        let currentBalance = 0;

        // Разбираем Data, чтобы получить текущий баланс
        if (lastEvent && lastEvent.Data) {
            try {
                const data = JSON.parse(lastEvent.Data);
                if (data.NewBalance !== undefined) {
                    currentBalance = data.NewBalance;
                } else if (data.Balance !== undefined) {
                    currentBalance = data.Balance;
                }
            } catch (e) {
                // Игнорируем ошибки разбора
            }
        }

        const eventsToInsert = [];
        const now = new Date();
        const batchId = uuidv4();
        const timestampEpoch = Math.floor(now.getTime() / 1000);
        const baseId = Date.now();

        // 1. Если агрегата ещё нет, создаём StarsAccountCreatedEvent
        if (currentSequence === 0) {
            currentSequence++;

            const createEventData = {
                PeerId: parseInt(userId),
                RequestInfo: {
                    DeviceType: 1,
                    AddRequestIdToCache: true
                }
            };

            const createMetadata = {
                timestamp: now.toISOString(),
                aggregate_sequence_number: currentSequence.toString(),
                aggregate_name: 'StarsAggregate',
                aggregate_id: aggregateId,
                event_id: `event-${new ObjectId().toString()}`,
                timestamp_epoch: timestampEpoch.toString(),
                batch_id: batchId,
                source_id: `command-${new ObjectId().toString()}`,
                event_name: 'StarsAccountCreatedEvent',
                event_version: '1'
            };

            eventsToInsert.push({
                _id: baseId,
                BatchId: batchId,
                AggregateId: aggregateId,
                AggregateName: 'StarsAggregate',
                AggregateSequenceNumber: currentSequence,
                Data: JSON.stringify(createEventData),
                Metadata: JSON.stringify(createMetadata)
            });

            console.log(`Creating new StarsAggregate for user ${userId}`);
        }

        // 2. Создаём StarsAddedEvent
        currentSequence++;
        const newBalance = currentBalance + parseInt(amount);
        const transactionId = uuidv4();

        const eventData = {
            PeerId: parseInt(userId),
            Amount: parseInt(amount),
            TransactionId: transactionId,
            Reason: reason || 'Admin issued stars',
            NewBalance: newBalance,
            RequestInfo: {
                DeviceType: 1,
                AddRequestIdToCache: true
            }
        };

        const metadata = {
            timestamp: now.toISOString(),
            aggregate_sequence_number: currentSequence.toString(),
            aggregate_name: 'StarsAggregate',
            aggregate_id: aggregateId,
            event_id: `event-${new ObjectId().toString()}`,
            timestamp_epoch: timestampEpoch.toString(),
            batch_id: batchId,
            source_id: `command-${new ObjectId().toString()}`,
            event_name: 'StarsAddedEvent',
            event_version: '1'
        };

        eventsToInsert.push({
            _id: baseId + eventsToInsert.length, // сдвигаем, если уже добавили StarsAccountCreatedEvent
            BatchId: batchId,
            AggregateId: aggregateId,
            AggregateName: 'StarsAggregate',
            AggregateSequenceNumber: currentSequence,
            Data: JSON.stringify(eventData),
            Metadata: JSON.stringify(metadata)
        });

        // Сохраняем события
        if (eventsToInsert.length > 0) {
            await eventsCollection.insertMany(eventsToInsert);
            console.log(`Inserted StarsAddedEvent for user ${userId}: +${amount} stars, new balance: ${newBalance}`);
        }

        // --- Обновление read-модели (для немедленного отклика интерфейса) ---
        // Это может дублировать обновление от EventFlow, но повышает отзывчивость UI

        const transaction = {
            Id: new ObjectId().toString(),
            Amount: parseInt(amount),
            Date: now,
            Type: 'TopUp',
            Reason: reason || 'Admin issued stars',
            TransactionId: transactionId
        };

        // Проверяем, есть ли read-модель (должна быть, если агрегат существует, но может быть не синхронизирована)
        let starsAccount = await starsCollection.findOne({ _id: documentId });

        if (!starsAccount) {
            // Создаём новый счёт звёзд в read-модели
            starsAccount = {
                _id: documentId,
                Id: documentId,
                PeerId: parseInt(userId), // храним как обычный int64, не Long
                Balance: newBalance, // храним как обычный int64, не Long
                Transactions: [transaction],
                Version: Long.fromNumber(1)
            };
            await starsCollection.insertOne(starsAccount);
            console.log(`Created new StarsReadModel with PeerId=${userId} (as int64)`);
        } else {
            // Обновляем существующий счёт
            const currentBalance = starsAccount.Balance instanceof Long
                ? starsAccount.Balance.toNumber()
                : parseInt(starsAccount.Balance);

            await starsCollection.updateOne(
                { _id: documentId },
                {
                    $set: {
                        Balance: currentBalance + parseInt(amount), // храним как обычный int64
                        PeerId: parseInt(userId) // следим, чтобы PeerId был обычным int64
                    },
                    $push: { Transactions: transaction }
                }
            );
            console.log(`Updated StarsReadModel with PeerId=${userId}, newBalance=${currentBalance + parseInt(amount)}`);
        }

        res.json({
            success: true,
            data: {
                userId: parseInt(userId),
                userName: user.UserName || null,
                firstName: user.FirstName || null,
                newBalance: newBalance.toString(),
                transaction: {
                    amount: parseInt(amount),
                    reason: reason || 'Admin issued stars',
                    date: transaction.Date
                }
            }
        });
    } catch (error) {
        console.error('Error issuing stars:', error);
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/stars/user/:userId - баланс звёзд и транзакции пользователя
router.get('/user/:userId', async (req, res) => {
    try {
        const { userId } = req.params;
        const db = req.db;

        // Получаем сведения о пользователе
        const userCollection = db.collection('eventflow-userreadmodel');
        const user = await userCollection.findOne({ UserId: parseInt(userId) });

        if (!user) {
            return res.status(404).json({
                success: false,
                error: 'User not found'
            });
        }

        // Пытаемся найти события StarsAggregate для этого пользователя
        const eventsCollection = db.collection('eventflow.events');
        const existingEvent = await eventsCollection.findOne({
            AggregateName: 'StarsAggregate',
            'Data': { $regex: `"PeerId":${userId}` }
        });

        let starsAccount = null;
        if (existingEvent) {
            const aggregateId = existingEvent.AggregateId;
            const starsCollection = db.collection('eventflow-starsreadmodel');
            starsAccount = await starsCollection.findOne({ _id: aggregateId });
        }

        res.json({
            success: true,
            data: {
                user: {
                    userId: user.UserId,
                    userName: user.UserName || null,
                    firstName: user.FirstName || null,
                    lastName: user.LastName || null,
                    phoneNumber: user.PhoneNumber || null
                },
                balance: starsAccount?.Balance?.toString() || "0",
                transactions: starsAccount?.Transactions || []
            }
        });
    } catch (error) {
        console.error('Error getting user stars:', error);
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/stars/recent - последние транзакции звёзд по всем пользователям
router.get('/recent', async (req, res) => {
    try {
        const { limit = 20 } = req.query;
        const db = req.db;

        const starsCollection = db.collection('eventflow-starsreadmodel');

        // Собираем последние транзакции
        const accounts = await starsCollection
            .find({ Transactions: { $exists: true, $ne: [] } })
            .limit(parseInt(limit))
            .sort({ 'Transactions.Date': -1 })
            .toArray();

        // Разворачиваем транзакции в плоский список
        const recentTransactions = [];
        for (const account of accounts) {
            if (account.Transactions) {
                for (const tx of account.Transactions.slice(-5)) { // последние 5 на пользователя
                    recentTransactions.push({
                        userId: account.PeerId.toString(),
                        ...tx
                    });
                }
            }
        }

        // Сортируем по дате
        recentTransactions.sort((a, b) => new Date(b.Date) - new Date(a.Date));

        res.json({
            success: true,
            data: recentTransactions.slice(0, parseInt(limit))
        });
    } catch (error) {
        console.error('Error getting recent transactions:', error);
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// POST /api/stars/notify-gift-received - уведомить пользователя о полученном подарке
router.post('/notify-gift-received', async (req, res) => {
    try {
        const { recipientUserId, senderUserId, giftId, stars } = req.body;

        if (!recipientUserId || !senderUserId || !giftId) {
            return res.status(400).json({
                success: false,
                error: 'recipientUserId, senderUserId, and giftId are required'
            });
        }

        const db = req.db;
        
        // Получаем сведения о получателе
        const userCollection = db.collection('eventflow-userreadmodel');
        const recipient = await userCollection.findOne({ UserId: parseInt(recipientUserId) });
        const sender = await userCollection.findOne({ UserId: parseInt(senderUserId) });

        if (!recipient) {
            return res.status(404).json({
                success: false,
                error: 'Recipient not found'
            });
        }

        console.log(`Notification requested for user ${recipientUserId}: Gift ${giftId} from user ${senderUserId}`);

        // Само уведомление отправляет MessageDomainEventHandler при обработке
        // InboxMessageCreatedEvent. Этот эндпоинт нужен для тестов и отладки.

        res.json({
            success: true,
            message: `Notification logged for user ${recipientUserId}`,
            data: {
                recipient: {
                    userId: recipient.UserId,
                    userName: recipient.UserName || null,
                    firstName: recipient.FirstName || null
                },
                sender: sender ? {
                    userId: sender.UserId,
                    userName: sender.UserName || null,
                    firstName: sender.FirstName || null
                } : null,
                giftId: parseInt(giftId),
                stars: parseInt(stars || 0)
            }
        });
    } catch (error) {
        console.error('Error processing gift notification:', error);
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

export default router;
