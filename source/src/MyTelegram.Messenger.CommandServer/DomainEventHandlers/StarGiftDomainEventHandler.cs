using MongoDB.Bson;
using MongoDB.Driver;
using MyTelegram.Domain.Aggregates.Dialog;
using MyTelegram.Domain.Aggregates.Messaging;
using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.Dialog;
using MyTelegram.Domain.Commands.Messaging;
using MyTelegram.Domain.Commands.StarGift;
using MyTelegram.Domain.Events.StarGift;
using MyTelegram.Messenger.Services;
using MyTelegram.Schema;
using MyTelegram.Schema.Extensions;
using MyTelegram.Schema.Messages;
using System.Text;
using MyTelegram.Services.Services;

namespace MyTelegram.Messenger.CommandServer.DomainEventHandlers;

public class StarGiftDomainEventHandler(
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    ILogger<StarGiftDomainEventHandler> logger,
    IMongoDatabase mongoDatabase,
    IStarGiftAttributeGenerator attributeGenerator,
    IObjectMessageSender objectMessageSender)
    : ISubscribeSynchronousTo<StarGiftAggregate, StarGiftId, StarGiftSentEvent>,
      ISubscribeSynchronousTo<StarGiftAggregate, StarGiftId, StarGiftUpgradedEvent>,
      ISubscribeSynchronousTo<StarGiftAggregate, StarGiftId, StarGiftTransferredEvent>
{
    public async Task HandleAsync(
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftSentEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var aggregateEvent = domainEvent.AggregateEvent;

        Console.WriteLine($"[StarGiftDomainEventHandler] StarGiftSentEvent received! From={aggregateEvent.FromUserId} To={aggregateEvent.ToUserId} Gift={aggregateEvent.GiftId} MessageId={aggregateEvent.MessageId}");
        logger.LogInformation("StarGiftSentEvent: From={FromUserId} To={ToUserId} Gift={GiftId} MessageId={MessageId}",
            aggregateEvent.FromUserId, aggregateEvent.ToUserId, aggregateEvent.GiftId, aggregateEvent.MessageId);

        // Подгружаем полный документ стикера из базы данных
        TDocument? stickerDocument = null;
        if (aggregateEvent.GiftSticker != null && aggregateEvent.GiftSticker.Length >= 8)
        {
            var stickerId = BitConverter.ToInt64(aggregateEvent.GiftSticker, 0);
            Console.WriteLine($"[StarGiftDomainEventHandler] Loading sticker document: StickerId={stickerId}");
            stickerDocument = await LoadStickerDocumentAsync(stickerId, cancellationToken);

            if (stickerDocument == null)
            {
                Console.WriteLine($"[StarGiftDomainEventHandler] Sticker {stickerId} NOT FOUND in database, using placeholder");
                logger.LogWarning("Sticker {StickerId} not found in database, using placeholder", stickerId);
            }
            else
            {
                Console.WriteLine($"[StarGiftDomainEventHandler] Sticker {stickerId} loaded successfully with AccessHash={stickerDocument.AccessHash}");
            }
        }
        else
        {
            Console.WriteLine($"[StarGiftDomainEventHandler] GiftSticker is null or empty!");
        }

        // Формируем сообщение с messageActionStarGift
        var messageAction = new TMessageActionStarGift
        {
            NameHidden = aggregateEvent.NameHidden,
            Saved = false,
            Converted = false,
            Upgraded = false,
            Refunded = false,
            CanUpgrade = aggregateEvent.CanUpgrade,
            Gift = new TStarGift
            {
                Limited = false,
                SoldOut = false,
                Birthday = false,
                Id = aggregateEvent.GiftId,
                Sticker = stickerDocument ?? new TDocument
                {
                    Id = aggregateEvent.GiftSticker != null && aggregateEvent.GiftSticker.Length > 0 
                        ? BitConverter.ToInt64(aggregateEvent.GiftSticker, 0) 
                        : aggregateEvent.GiftId,
                    AccessHash = 0,
                    FileReference = aggregateEvent.GiftSticker ?? Array.Empty<byte>(),
                    Date = aggregateEvent.Date,
                    MimeType = "application/x-tgsticker",
                    Size = 0,
                    DcId = 2,
                    Attributes = new TVector<IDocumentAttribute>(
                        new TDocumentAttributeSticker
                        {
                            Alt = "🎁",
                            Stickerset = new TInputStickerSetEmpty()
                        },
                        new TDocumentAttributeAnimated()
                    )
                },
                Stars = aggregateEvent.Stars,
                AvailabilityRemains = null,
                AvailabilityTotal = null,
                ConvertStars = aggregateEvent.ConvertStars,
                FirstSaleDate = null,
                LastSaleDate = null,
                UpgradeStars = aggregateEvent.UpgradeStars,
                Title = null
            },
            Message = aggregateEvent.Message != null 
                ? new TTextWithEntities
                {
                    Text = aggregateEvent.Message,
                    Entities = new TVector<IMessageEntity>()
                }
                : null,
            ConvertStars = aggregateEvent.ConvertStars,
            UpgradeStars = aggregateEvent.UpgradeStars
        };

        // Готовим входящее сообщение для получателя
        var toPeer = aggregateEvent.ToPeerId.HasValue
            ? new Peer(PeerType.Channel, aggregateEvent.ToPeerId.Value)
            : new Peer(PeerType.User, aggregateEvent.ToUserId);

        var fromPeer = new Peer(PeerType.User, aggregateEvent.FromUserId);

        // Используем MessageId, зарезервированный на этапе инициации подарка.
        // Так Вы избегаете лишней генерации идентификатора и возможных коллизий
        var inboxMessageId = aggregateEvent.MessageId;

        // Генерируем Pts для входящего сообщения (получатель)
        var inboxPts = await idGenerator.NextIdAsync(IdType.Pts, aggregateEvent.ToUserId, 1);
        Console.WriteLine($"[StarGiftDomainEventHandler] Using reserved MessageId={inboxMessageId} and generated Pts={inboxPts} for recipient: ToUserId={aggregateEvent.ToUserId}");

        // Для входящего: OwnerPeer = получатель (toPeer), ToPeer = отправитель (fromPeer), SenderPeer = отправитель (fromPeer).
        // В итоге во входящих получателя появляется сообщение о подарке ОТ отправителя
        var messageItem = new MessageItem(
            toPeer,   // OwnerPeerId - recipient who owns this inbox message
            fromPeer, // ToPeer - in recipient's view, the "other peer" is the sender
            fromPeer, // SenderPeerId - sender of the gift
            aggregateEvent.FromUserId,
            inboxMessageId, // Используем уникальный MessageId для каждого получателя
            string.Empty,
            aggregateEvent.Date,
            0,
            false,
            SendMessageType.MessageService,
            MessageType.Text,
            MessageSubType.Normal,
            MessageAction: messageAction,
            MessageActionType: MessageActionType.CustomAction,
            Pts: inboxPts
        );

        // Берём MessageId в рамках конкретного peer, чтобы получить уникальный идентификатор агрегата
        var inboxMessageAggregateId = MessageId.Create(toPeer.PeerId, inboxMessageId);

        // Проверяем, не создано ли сообщение ранее (идемпотентность)
        var existingInboxMessage = await mongoDatabase.GetCollection<BsonDocument>("ReadModel-MessageReadModel")
            .Find(Builders<BsonDocument>.Filter.Eq("_id", inboxMessageAggregateId.Value))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (existingInboxMessage == null)
        {
            var createInboxCommand = new CreateInboxMessageCommand(
                inboxMessageAggregateId,
                aggregateEvent.RequestInfo,
                messageItem,
                inboxMessageId
            );

            Console.WriteLine($"[StarGiftDomainEventHandler] Publishing CreateInboxMessageCommand for MessageId={inboxMessageId}");
            await commandBus.PublishAsync(createInboxCommand, cancellationToken);
            Console.WriteLine($"[StarGiftDomainEventHandler] Inbox message created");
        }
        else
        {
            Console.WriteLine($"[StarGiftDomainEventHandler] Inbox message {inboxMessageId} already exists, skipping creation (idempotent)");
            logger.LogWarning("Inbox message {MessageId} already exists for gift, skipping creation", inboxMessageId);
        }

        // Создаём или обновляем диалог для входящего сообщения, чтобы увеличить счётчик непрочитанных (только если сообщение было создано)
        if (existingInboxMessage == null)
        {
            var receiveInboxCommand = new ReceiveInboxMessageCommand(
                DialogId.Create(messageItem.OwnerPeer.PeerId, messageItem.ToPeer),
                aggregateEvent.RequestInfo,
                messageItem.MessageId,
                messageItem.OwnerPeer.PeerId,
                messageItem.ToPeer,
                null // senderDefaultHistoryTTL
            );
            
            Console.WriteLine($"[StarGiftDomainEventHandler] Publishing ReceiveInboxMessageCommand to create/update Dialog");
            await commandBus.PublishAsync(receiveInboxCommand, cancellationToken);
            Console.WriteLine($"[StarGiftDomainEventHandler] Dialog updated for inbox message");

            logger.LogInformation("Star gift inbox message created: MessageId={MessageId} To={ToUserId}",
                inboxMessageId, aggregateEvent.ToUserId);
        }

        // Создаём исходящее сообщение для отправителя с другим MessageId.
        // Счётчик MessageId ведётся для каждого peer отдельно, что гарантирует уникальность в пространстве сообщений отправителя
        var outboxMessageId = await idGenerator.NextIdAsync(IdType.MessageId, aggregateEvent.FromUserId, 1);

        // Генерируем Pts для исходящего сообщения (отправитель)
        var outboxPts = await idGenerator.NextIdAsync(IdType.Pts, aggregateEvent.FromUserId, 1);
        Console.WriteLine($"[StarGiftDomainEventHandler] Generated MessageId={outboxMessageId} and Pts={outboxPts} for sender: FromUserId={aggregateEvent.FromUserId}");

        // Для исходящего: OwnerPeer = отправитель (fromPeer), ToPeer = получатель (toPeer), SenderPeer = отправитель (fromPeer).
        // В итоге в исходящих отправителя появляется сообщение о подарке ДЛЯ получателя
        var outboxMessageItem = new MessageItem(
            fromPeer, // OwnerPeerId - sender who owns this outbox message
            toPeer,   // ToPeer - recipient of the gift
            fromPeer, // SenderPeerId - sender of the gift
            aggregateEvent.FromUserId,
            outboxMessageId,
            string.Empty,
            aggregateEvent.Date,
            0,
            true,
            SendMessageType.MessageService,
            MessageType.Text,
            MessageSubType.Normal,
            MessageAction: messageAction,
            MessageActionType: MessageActionType.CustomAction,
            Pts: outboxPts
        );

        // Берём MessageId в рамках конкретного peer, чтобы получить уникальный идентификатор агрегата
        var outboxMessageAggregateId = MessageId.Create(fromPeer.PeerId, outboxMessageId);

        // Проверяем, не создано ли исходящее сообщение ранее (идемпотентность)
        var existingOutboxMessage = await mongoDatabase.GetCollection<BsonDocument>("ReadModel-MessageReadModel")
            .Find(Builders<BsonDocument>.Filter.Eq("_id", outboxMessageAggregateId.Value))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (existingOutboxMessage == null)
        {
            var createOutboxCommand = new CreateOutboxMessageCommand(
                outboxMessageAggregateId,
                aggregateEvent.RequestInfo,
                outboxMessageItem,
                mentionedUserIds: null,
                replyToMsgItems: null,
                clearDraft: true,
                groupItemCount: 1,
                linkedChannelId: null,
                chatMembers: null
            );

            Console.WriteLine($"[StarGiftDomainEventHandler] Publishing CreateOutboxMessageCommand for MessageId={outboxMessageId}");
            await commandBus.PublishAsync(createOutboxCommand, cancellationToken);
            Console.WriteLine($"[StarGiftDomainEventHandler] Outbox message created");
        }
        else
        {
            Console.WriteLine($"[StarGiftDomainEventHandler] Outbox message {outboxMessageId} already exists, skipping creation (idempotent)");
            logger.LogWarning("Outbox message {MessageId} already exists for gift, skipping creation", outboxMessageId);
        }

        // Создаём или обновляем диалог для исходящего сообщения (только если сообщение было создано)
        if (existingOutboxMessage == null)
        {
            var updateOutboxTopMessageCommand = new SetOutboxTopMessageCommand(
                DialogId.Create(outboxMessageItem.OwnerPeer.PeerId, outboxMessageItem.ToPeer),
                outboxMessageItem.MessageId,
                outboxMessageItem.OwnerPeer.PeerId,
                outboxMessageItem.ToPeer,
                false // clearDraft
            );
            
            Console.WriteLine($"[StarGiftDomainEventHandler] Publishing SetOutboxTopMessageCommand to update Dialog");
            await commandBus.PublishAsync(updateOutboxTopMessageCommand, cancellationToken);
            Console.WriteLine($"[StarGiftDomainEventHandler] Dialog updated for outbox message");

            logger.LogInformation("Star gift outbox message created: MessageId={MessageId} From={FromUserId}",
                outboxMessageId, aggregateEvent.FromUserId);

            // Сохраняем идентификаторы входящего и исходящего сообщений в агрегат StarGift для последующего поиска.
        // Идентификаторы обновляются через команды, поэтому ReadModel обновляется надёжно событиями и без гонок
        var setInboxCommand = new SetStarGiftInboxMessageIdCommand(domainEvent.AggregateIdentity, inboxMessageId);
        var setOutboxCommand = new SetStarGiftOutboxMessageIdCommand(domainEvent.AggregateIdentity, outboxMessageId);

        Console.WriteLine($"[StarGiftDomainEventHandler] Dispatching ID update commands for gift: Inbox={inboxMessageId}, Outbox={outboxMessageId}");
        await commandBus.PublishAsync(setInboxCommand, cancellationToken);
        await commandBus.PublishAsync(setOutboxCommand, cancellationToken);
        }

        // Уменьшаем остаток для лимитированных подарков
        await DecreaseGiftSupplyAsync(aggregateEvent.GiftId, cancellationToken);
    }

    private async Task DecreaseGiftSupplyAsync(long giftId, CancellationToken cancellationToken)
    {
        try
        {
            var collection = mongoDatabase.GetCollection<BsonDocument>("AvailableStarGiftReadModel");

            // Ищем подарок в каталоге
            var filter = Builders<BsonDocument>.Filter.Eq("GiftId", giftId);
            var gift = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);

            if (gift == null)
            {
                logger.LogWarning("Gift {GiftId} not found in catalog, cannot decrease supply", giftId);
                return;
            }

            // Обновляем даты продаж для всех подарков (лимитированных и нелимитированных)
            var currentTimestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var firstSaleDate = gift.Contains("FirstSaleDate") && !gift["FirstSaleDate"].IsBsonNull
                ? gift["FirstSaleDate"].ToInt32()
                : (int?)null;

            var updateBuilder = Builders<BsonDocument>.Update
                .Set("LastSaleDate", currentTimestamp);

            // Проставляем FirstSaleDate только если это первая продажа
            if (!firstSaleDate.HasValue)
            {
                updateBuilder = updateBuilder.Set("FirstSaleDate", currentTimestamp);
                logger.LogInformation("First sale of gift {GiftId}! FirstSaleDate set to {Date}", giftId, currentTimestamp);
            }

            // Проверяем, лимитированный ли это подарок
            var isLimited = gift.GetValue("Limited", false).AsBoolean;
            if (!isLimited)
            {
                // Для нелимитированных подарков просто обновляем даты продаж
                await collection.UpdateOneAsync(filter, updateBuilder, cancellationToken: cancellationToken);
                logger.LogDebug("Gift {GiftId} is not limited, sale dates updated", giftId);
                return;
            }

            // Уменьшаем AvailabilityRemains для лимитированных подарков
            var currentRemains = gift.GetValue("AvailabilityRemains", 0).ToInt32();
            if (currentRemains > 0)
            {
                updateBuilder = updateBuilder
                    .Set("AvailabilityRemains", currentRemains - 1)
                    .Set("SoldOut", currentRemains - 1 <= 0);

                await collection.UpdateOneAsync(filter, updateBuilder, cancellationToken: cancellationToken);

                logger.LogInformation("Gift {GiftId} supply decreased: {OldRemains} -> {NewRemains}, LastSaleDate updated",
                    giftId, currentRemains, currentRemains - 1);

                if (currentRemains - 1 <= 0)
                {
                    logger.LogInformation("Gift {GiftId} is now SOLD OUT!", giftId);
                }
            }
            else
            {
                logger.LogWarning("Gift {GiftId} already has 0 supply!", giftId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to decrease supply for gift {GiftId}", giftId);
            // Исключение не пробрасываем: для процесса отправки подарка это некритично
        }
    }

    private async Task<TDocument?> LoadStickerDocumentAsync(long stickerId, CancellationToken cancellationToken)
    {
        try
        {
            // Сначала пробуем ReadModel-DocumentReadModel (там лежат стикеры подарков)
            var collection = mongoDatabase.GetCollection<BsonDocument>("ReadModel-DocumentReadModel");
            var filter = Builders<BsonDocument>.Filter.Eq("DocumentId", stickerId);
            var document = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);

            if (document == null)
            {
                logger.LogWarning("Document {StickerId} not found in ReadModel-DocumentReadModel, trying eventflow-documentreadmodel", stickerId);

                // Пробуем eventflow-documentreadmodel
                var eventflowCollection = mongoDatabase.GetCollection<BsonDocument>("eventflow-documentreadmodel");
                document = await eventflowCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

                if (document == null)
                {
                    logger.LogWarning("Document {StickerId} not found in eventflow-documentreadmodel, trying custom_emoji_documents", stickerId);

                    // Пробуем коллекцию custom_emoji_documents
                    var emojiCollection = mongoDatabase.GetCollection<BsonDocument>("custom_emoji_documents");
                    var emojiFilter = Builders<BsonDocument>.Filter.Eq("_id", stickerId);
                    document = await emojiCollection.Find(emojiFilter).FirstOrDefaultAsync(cancellationToken);

                    if (document == null)
                    {
                        logger.LogError("Document {StickerId} not found in ANY collection!", stickerId);
                        return null;
                    }
                }
            }
            
            var tDocument = new TDocument
            {
                Id = document.GetValue("DocumentId", stickerId).ToInt64(),
                AccessHash = document.GetValue("AccessHash", 0L).ToInt64(),
                FileReference = document.GetValue("FileReference", BsonBinaryData.Create(Array.Empty<byte>())).AsByteArray,
                Date = document.GetValue("Date", 0).ToInt32(),
                MimeType = document.GetValue("MimeType", "application/x-tgsticker").AsString,
                Size = document.GetValue("Size", 0L).ToInt64(),
                DcId = document.GetValue("DcId", 2).ToInt32(),
                Attributes = new TVector<IDocumentAttribute>()
            };
            
            // Разбираем Attributes2
            if (document.Contains("Attributes2") && document["Attributes2"].IsBsonArray)
            {
                var attributes = document["Attributes2"].AsBsonArray;
                foreach (var attr in attributes)
                {
                    if (attr.IsBsonDocument)
                    {
                        var attrDoc = attr.AsBsonDocument;
                        var constructorId = attrDoc.GetValue("ConstructorId", 0).ToInt32();

                        // Разбираем по ConstructorId
                        IDocumentAttribute? parsedAttr = constructorId switch
                        {
                            0x6319d612 => new TDocumentAttributeSticker
                            {
                                Alt = attrDoc.GetValue("Alt", "").AsString,
                                Stickerset = new TInputStickerSetEmpty()
                            },
                            0x11b58939 => new TDocumentAttributeAnimated(),
                            0x0ef02ce6 => new TDocumentAttributeImageSize
                            {
                                W = attrDoc.GetValue("W", 0).ToInt32(),
                                H = attrDoc.GetValue("H", 0).ToInt32()
                            },
                            _ => null
                        };
                        
                        if (parsedAttr != null)
                        {
                            tDocument.Attributes.Add(parsedAttr);
                        }
                    }
                }
            }
            
            logger.LogInformation("Loaded sticker document {StickerId} with {AttributeCount} attributes", stickerId, tDocument.Attributes.Count);
            return tDocument;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load sticker document {StickerId}", stickerId);
            return null;
        }
    }


    public async Task HandleAsync(
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftUpgradedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var aggregateEvent = domainEvent.AggregateEvent;
        var aggregateId = domainEvent.AggregateIdentity.Value;

        logger.LogInformation("Star gift upgraded: AggregateId={AggregateId}, UpgradeMsgId={UpgradeMsgId}",
            aggregateId, aggregateEvent.UpgradeMsgId);

        Console.WriteLine($"[StarGiftUpgradedEvent] Handler called! AggregateId={aggregateId}, UpgradeMsgId={aggregateEvent.UpgradeMsgId}");

        // Загружаем данные подарка из ReadModel, чтобы получить FromUserId, ToUserId, GiftId
        var collection = mongoDatabase.GetCollection<BsonDocument>("eventflow-stargiftreadmodel");
        var filter = Builders<BsonDocument>.Filter.Eq("AggregateId", aggregateId);
        var giftDoc = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);

        if (giftDoc == null)
        {
            logger.LogError("Gift not found in ReadModel: AggregateId={AggregateId}", aggregateId);
            return;
        }
        
        var fromUserId = giftDoc.GetValue("FromUserId", 0L).ToInt64();
        var toUserId = giftDoc.GetValue("ToUserId", 0L).ToInt64();
        var giftId = giftDoc.GetValue("GiftId", 0L).ToInt64();
        var uniqueSlug = giftDoc.Contains("UniqueSlug") && !giftDoc["UniqueSlug"].IsBsonNull 
            ? giftDoc["UniqueSlug"].AsString 
            : null;
        var uniqueNum = giftDoc.GetValue("UniqueNum", 0).ToInt32();
        var attributesBytes = giftDoc.Contains("Attributes") && !giftDoc["Attributes"].IsBsonNull 
            ? giftDoc["Attributes"].AsByteArray 
            : null;
        
        // Проверяем, относится ли апгрейд к другому пользователю.
        // Если FromUserId == 0 или FromUserId == ToUserId, это подарок самому себе, сообщения не создаём
        if (fromUserId == 0 || fromUserId == toUserId)
        {
            Console.WriteLine($"[StarGiftUpgradedEvent] Self-gift or anonymous gift, skipping chat message creation");
            return;
        }

        Console.WriteLine($"[StarGiftUpgradedEvent] Creating upgrade messages: From={fromUserId}, To={toUserId}, GiftId={giftId}");

        // Десериализуем атрибуты
        TVector<IStarGiftAttribute>? attributes = null;
        if (attributesBytes != null && attributesBytes.Length > 0)
        {
            try
            {
                attributes = attributesBytes.ToTObject<TVector<IStarGiftAttribute>>();
                Console.WriteLine($"Deserialized {attributes?.Count ?? 0} attributes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to deserialize attributes: {ex.Message}");
            }
        }

        // Загружаем сведения о подарке из каталога
        var catalogCollection = mongoDatabase.GetCollection<BsonDocument>("AvailableStarGiftReadModel");
        var catalogFilter = Builders<BsonDocument>.Filter.Eq("GiftId", giftId);
        var catalogDoc = await catalogCollection.Find(catalogFilter).FirstOrDefaultAsync(cancellationToken);
        
        var title = catalogDoc?.GetValue("Title", $"Gift #{giftId}").AsString ?? $"Gift #{giftId}";
        var availabilityTotal = catalogDoc?.GetValue("AvailabilityTotal", 1000).ToInt32() ?? 1000;
        var availabilityRemains = catalogDoc?.GetValue("AvailabilityRemains", 0).ToInt32() ?? 0;
        var availabilityIssued = availabilityTotal - availabilityRemains;
        
        // Формируем TStarGiftUnique для action сообщения
        var uniqueIdHash = aggregateId.GetHashCode();
        var uniqueGiftId = Math.Abs((long)uniqueIdHash) + (giftId * 1000000);
        
        var uniqueGift = new TStarGiftUnique
        {
            Id = uniqueGiftId,
            Title = title,
            Slug = uniqueSlug ?? $"gift-{uniqueNum}",
            Num = uniqueNum,
            OwnerId = new TPeerUser { UserId = toUserId },
            Attributes = attributes ?? new TVector<IStarGiftAttribute>(),
            AvailabilityIssued = availabilityIssued,
            AvailabilityTotal = availabilityTotal
        };
        uniqueGift.ComputeFlag();

        // Формируем action сообщения
        var messageAction = new TMessageActionStarGiftUnique
        {
            Upgrade = true,
            Saved = false,
            Transferred = false,
            Refunded = false,
            FromId = new TPeerUser { UserId = toUserId },  // Тот, кто выполнил апгрейд
            Peer = new TPeerUser { UserId = fromUserId },   // Тот, кто подарил
            SavedId = null,
            Gift = uniqueGift
        };
        messageAction.ComputeFlag();

        // Готовим peer-ы
        var fromPeer = new Peer(PeerType.User, fromUserId);  // Даритель
        var toPeer = new Peer(PeerType.User, toUserId);      // Тот, кто выполнил апгрейд

        // Генерируем MessageId
        var inboxGlobalMessageId = await idGenerator.NextLongIdAsync(IdType.MessageId, 0);
        var inboxMessageId = (int)inboxGlobalMessageId;
        var outboxGlobalMessageId = await idGenerator.NextLongIdAsync(IdType.MessageId, 0);
        var outboxMessageId = (int)outboxGlobalMessageId;

        // Генерируем Pts
        var gifterPts = await idGenerator.NextIdAsync(IdType.Pts, fromUserId, 1);
        var upgraderPts = await idGenerator.NextIdAsync(IdType.Pts, toUserId, 1);

        Console.WriteLine($"Generated MessageIds: Inbox={inboxMessageId}, Outbox={outboxMessageId}");

        // Создаём ВХОДЯЩЕЕ сообщение для ДАРИТЕЛЯ (он получает уведомление, что его подарок улучшили)
        var inboxMessageItem = new MessageItem(
            fromPeer,  // OwnerPeerId - входящее сообщение принадлежит дарителю
            toPeer,    // ToPeer - сообщение "от" того, кто выполнил апгрейд
            toPeer,    // SenderPeerId - отправитель - тот, кто выполнил апгрейд
            toUserId,  // SenderUserId
            inboxMessageId,
            string.Empty,
            aggregateEvent.UpgradeDate,
            0,
            false,     // Для дарителя сообщение не исходящее
            SendMessageType.MessageService,
            MessageType.Text,
            MessageSubType.Normal,
            MessageAction: messageAction,
            MessageActionType: MessageActionType.CustomAction,
            Pts: gifterPts
        );
        
        var inboxMessageAggregateId = MessageId.Create(fromPeer.PeerId, inboxMessageId);

        // Проверяем, не создано ли входящее сообщение ранее (идемпотентность)
        var existingInboxMsg = await mongoDatabase.GetCollection<BsonDocument>("ReadModel-MessageReadModel")
            .Find(Builders<BsonDocument>.Filter.Eq("_id", inboxMessageAggregateId.Value))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (existingInboxMsg == null)
        {
            var createInboxCommand = new CreateInboxMessageCommand(
                inboxMessageAggregateId,
                aggregateEvent.RequestInfo,
                inboxMessageItem,
                (int)inboxGlobalMessageId
            );
            
            await commandBus.PublishAsync(createInboxCommand, cancellationToken);
            Console.WriteLine($"Inbox message created for gifter (user {fromUserId})");

            // Обновляем диалог дарителя
            var receiveInboxCommand = new ReceiveInboxMessageCommand(
                DialogId.Create(fromPeer.PeerId, toPeer),
                aggregateEvent.RequestInfo,
                inboxMessageId,
                fromPeer.PeerId,
                toPeer,
                null
            );
            await commandBus.PublishAsync(receiveInboxCommand, cancellationToken);
        }
        else
        {
            Console.WriteLine($"Inbox message {inboxMessageId} already exists for upgrade, skipping (idempotent)");
        }

        // Создаём ИСХОДЯЩЕЕ сообщение для того, кто выполнил апгрейд (он видит результат апгрейда в своём чате)
        var outboxMessageItem = new MessageItem(
            toPeer,    // OwnerPeerId - исходящее сообщение принадлежит тому, кто выполнил апгрейд
            fromPeer,  // ToPeer - сообщение "для" дарителя
            toPeer,    // SenderPeerId - отправитель - тот, кто выполнил апгрейд
            toUserId,  // SenderUserId
            outboxMessageId,
            string.Empty,
            aggregateEvent.UpgradeDate,
            0,
            true,      // Для того, кто выполнил апгрейд, сообщение исходящее
            SendMessageType.MessageService,
            MessageType.Text,
            MessageSubType.Normal,
            MessageAction: messageAction,
            MessageActionType: MessageActionType.CustomAction,
            Pts: upgraderPts
        );
        
        var outboxMessageAggregateId = MessageId.Create(toPeer.PeerId, outboxMessageId);

        // Проверяем, не создано ли исходящее сообщение ранее (идемпотентность)
        var existingOutboxMsg = await mongoDatabase.GetCollection<BsonDocument>("ReadModel-MessageReadModel")
            .Find(Builders<BsonDocument>.Filter.Eq("_id", outboxMessageAggregateId.Value))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (existingOutboxMsg == null)
        {
            var createOutboxCommand = new CreateOutboxMessageCommand(
                outboxMessageAggregateId,
                aggregateEvent.RequestInfo,
                outboxMessageItem,
                mentionedUserIds: null,
                replyToMsgItems: null,
                clearDraft: false,
                groupItemCount: 1,
                linkedChannelId: null,
                chatMembers: null
            );
            
            await commandBus.PublishAsync(createOutboxCommand, cancellationToken);
            Console.WriteLine($"Outbox message created for upgrader (user {toUserId})");

            // Обновляем диалог того, кто выполнил апгрейд
            var updateOutboxTopMessageCommand = new SetOutboxTopMessageCommand(
                DialogId.Create(toPeer.PeerId, fromPeer),
                outboxMessageId,
                toPeer.PeerId,
                fromPeer,
                false
            );
            await commandBus.PublishAsync(updateOutboxTopMessageCommand, cancellationToken);
        }
        else
        {
            Console.WriteLine($"Outbox message {outboxMessageId} already exists for upgrade, skipping (idempotent)");
        }

        Console.WriteLine($"[StarGiftUpgradedEvent] Chat messages created for upgrade in dialog between {fromUserId} and {toUserId}");
        logger.LogInformation("Star gift upgrade completed: AggregateId={AggregateId}", aggregateId);
    }

    public async Task HandleAsync(
        IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftTransferredEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var aggregateEvent = domainEvent.AggregateEvent;
        var aggregateId = domainEvent.AggregateIdentity.Value;

        logger.LogInformation("Star gift transferred: AggregateId={AggregateId}, NewOwnerId={NewOwnerId}",
            aggregateId, aggregateEvent.NewOwnerId);

        Console.WriteLine($"[StarGiftTransferredEvent] Handler called! AggregateId={aggregateId}, NewOwnerId={aggregateEvent.NewOwnerId}");

        // Загружаем данные подарка из ReadModel
        var collection = mongoDatabase.GetCollection<BsonDocument>("eventflow-stargiftreadmodel");
        var filter = Builders<BsonDocument>.Filter.Eq("AggregateId", aggregateId);
        var giftDoc = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);

        if (giftDoc == null)
        {
            logger.LogError("Gift not found in ReadModel: AggregateId={AggregateId}", aggregateId);
            return;
        }

        var oldOwnerId = giftDoc.GetValue("ToUserId", 0L).ToInt64();
        var giftId = giftDoc.GetValue("GiftId", 0L).ToInt64();

        // Проверяем, является ли это передачей при перепродаже (подарок только что продали)
        var boughtByUserIdValue = giftDoc.GetValue("BoughtByUserId", BsonNull.Value);
        var isResale = !boughtByUserIdValue.IsBsonNull;

        // При перепродаже отправителем сообщения считается покупатель,
        // при обычной передаче - исходный отправитель
        var fromUserId = isResale
            ? boughtByUserIdValue.ToInt64() 
            : giftDoc.GetValue("FromUserId", 0L).ToInt64();
        var stars = giftDoc.GetValue("Stars", 0L).ToInt64();
        var convertStars = giftDoc.GetValue("ConvertStars", 0L).ToInt64();
        var canUpgrade = giftDoc.GetValue("CanUpgrade", false).ToBoolean();
        var upgraded = giftDoc.GetValue("Upgraded", false).ToBoolean();
        var nameHidden = giftDoc.GetValue("NameHidden", false).ToBoolean();
        var messageText = giftDoc.GetValue("Message", BsonNull.Value);
        var message = messageText.IsBsonNull ? null : messageText.AsString;
        var upgradeStars = giftDoc.GetValue("UpgradeStars", BsonNull.Value);
        var upgradeStarsValue = upgradeStars.IsBsonNull ? null : (long?)upgradeStars.ToInt64();
        
        // Читаем ResaleStars для передач при перепродаже.
        // ResaleStars - цена, уплаченная на маркетплейсе (например, 125),
        // Stars - исходная цена подарка (например, 500)
        var resaleStarsField = giftDoc.GetValue("ResaleStars", BsonNull.Value);
        var resaleStarsValue = resaleStarsField.IsBsonNull ? null : (long?)resaleStarsField.ToInt64();

        logger.LogInformation("Gift transferred from {OldOwner} to {NewOwner}, IsResale={IsResale}, Upgraded={Upgraded}, Stars={Stars}, ResaleStars={ResaleStars}",
            oldOwnerId, aggregateEvent.NewOwnerId, isResale, upgraded, stars, resaleStarsValue);
        Console.WriteLine($"[StarGiftTransferredEvent] Gift {giftId} transferred from {oldOwnerId} to {aggregateEvent.NewOwnerId}, IsResale={isResale}, Upgraded={upgraded}, Stars={stars} (original), ResaleStars={resaleStarsValue} (marketplace price)");

        // Подарки, выставленные на перепродажу, должны быть улучшенными
        if (isResale && !upgraded)
        {
            Console.WriteLine($"[StarGiftTransferredEvent] WARNING: Gift {giftId} is from resale but NOT upgraded! This is a data inconsistency!");
            Console.WriteLine($"This gift will show as regular gift instead of collectible. Check why non-upgraded gift was listed on resale.");
        }

        // Объявляем currentDate заранее, чтобы использовать его в messageAction
        var currentDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Загружаем документ стикера
        TDocument? stickerDocument = null;
        var stickerIdValue = giftDoc.GetValue("StickerDocumentId", BsonNull.Value);
        if (!stickerIdValue.IsBsonNull)
        {
            var stickerId = stickerIdValue.ToInt64();
            stickerDocument = await LoadStickerDocumentAsync(stickerId, cancellationToken);
        }
        
        // Формируем action сообщения для переданного подарка.
        // Для улучшенных подарков используем TMessageActionStarGiftUnique с ResaleAmount
        IMessageAction messageAction;

        if (upgraded)
        {
            // Читаем атрибуты из ReadModel, а не генерируем заново.
            // Атрибуты были созданы во время апгрейда и сохранены в базе
            TVector<IStarGiftAttribute>? giftAttributes = null;
            var attributesBytes = giftDoc.Contains("Attributes") && !giftDoc["Attributes"].IsBsonNull 
                ? giftDoc["Attributes"].AsByteArray 
                : null;
            
            if (attributesBytes != null && attributesBytes.Length > 0)
            {
                try
                {
                    giftAttributes = attributesBytes.ToTObject<TVector<IStarGiftAttribute>>();
                    Console.WriteLine($"[StarGiftTransferredEvent] Deserialized {giftAttributes?.Count ?? 0} attributes from ReadModel for gift {giftId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[StarGiftTransferredEvent] Failed to deserialize attributes: {ex.Message}");
                    // Запасной вариант: генерируем атрибуты заново
                    giftAttributes = attributeGenerator.GenerateAttributes(giftId);
                    Console.WriteLine($"[StarGiftTransferredEvent] Generated fallback {giftAttributes.Count} attributes for gift {giftId}");
                }
            }
            else
            {
                // В ReadModel атрибутов нет, генерируем заново
                Console.WriteLine($"[StarGiftTransferredEvent] No attributes found in ReadModel for gift {giftId}, generating new ones");
                giftAttributes = attributeGenerator.GenerateAttributes(giftId);
                Console.WriteLine($"[StarGiftTransferredEvent] Generated {giftAttributes.Count} attributes for gift {giftId}");
            }

            // Гарантируем наличие атрибутов (если null - используем пустой список)
            giftAttributes ??= new TVector<IStarGiftAttribute>();
            
            messageAction = new TMessageActionStarGiftUnique
            {
                Upgrade = true,
                Transferred = true, // This is a transfer
                Saved = false,
                Refunded = false,
                // При перепродаже задаём FromId (покупатель) и Peer (получатель)
                FromId = isResale ? new TPeerUser { UserId = fromUserId } : null,
                Peer = isResale ? new TPeerUser { UserId = aggregateEvent.NewOwnerId } : null,
                SavedId = isResale && (fromUserId != aggregateEvent.NewOwnerId) ? (long?)null : null, // Задаётся только если задан Peer
                // Используем resaleStarsValue (цену на маркетплейсе), а не stars (исходную цену).
                // Тогда tdesktop покажет "прислал уникальный коллекционный предмет за 125 звёзд", а не "за 500 звёзд"
                ResaleStars = isResale ? resaleStarsValue : null,
                Gift = new TStarGiftUnique
                {
                    Id = giftDoc.GetValue("CollectibleId", 0L).ToInt64(),
                    // Корректно обрабатываем значения BsonNull
                    Title = giftDoc.Contains("Title") && !giftDoc["Title"].IsBsonNull
                        ? giftDoc["Title"].AsString
                        : $"Gift #{giftId}",
                    Slug = giftDoc.Contains("UniqueSlug") && !giftDoc["UniqueSlug"].IsBsonNull
                        ? giftDoc["UniqueSlug"].AsString
                        : $"gift-{giftId}-{giftDoc.GetValue("UniqueNum", 0).ToInt32()}",
                    Num = giftDoc.GetValue("UniqueNum", 0).ToInt32(),
                    OwnerId = new TPeerUser { UserId = aggregateEvent.NewOwnerId },
                    // Используем сгенерированные атрибуты (model + pattern), а не пустой список.
                    // Атрибуты содержат Model (со стикером), Pattern, Backdrop и т.д.
                    // tdesktop проверяет, что model->document->sticker() и pattern->document->sticker() существуют
                    Attributes = giftAttributes,
                    AvailabilityIssued = 1,
                    AvailabilityTotal = 1
                }
            };
        }
        else
        {
            // Для не улучшенных подарков используем обычный TMessageActionStarGift
            messageAction = new TMessageActionStarGift
            {
                NameHidden = nameHidden,
                Saved = false,
                Converted = false,
                Upgraded = false,
                Refunded = false,
                CanUpgrade = canUpgrade,
                FromId = isResale ? new TPeerUser { UserId = fromUserId } : null,
                Peer = isResale ? new TPeerUser { UserId = aggregateEvent.NewOwnerId } : null,
                Gift = new TStarGift
                {
                    Limited = false,
                    SoldOut = false,
                    Birthday = false,
                    Id = giftId,
                    Sticker = (IDocument?)(stickerDocument ?? (IDocument)new TDocumentEmpty()),
                    Stars = stars,
                    AvailabilityRemains = null,
                    AvailabilityTotal = null,
                    ConvertStars = convertStars,
                    FirstSaleDate = null,
                    LastSaleDate = null,
                    UpgradeStars = upgradeStarsValue,
                    Title = null
                },
                Message = message != null 
                    ? new TTextWithEntities { Text = message, Entities = new TVector<IMessageEntity>() }
                    : null,
                ConvertStars = convertStars,
                UpgradeStars = upgradeStarsValue
            };
        }
        
        // Создаём входящее сообщение для нового владельца (получателя)
        var toPeer = new Peer(PeerType.User, aggregateEvent.NewOwnerId);
        var fromPeer = new Peer(PeerType.User, fromUserId); // Покупатель/отправитель

        var inboxMessageId = await idGenerator.NextIdAsync(IdType.MessageId, aggregateEvent.NewOwnerId, 1);
        var inboxPts = await idGenerator.NextIdAsync(IdType.Pts, aggregateEvent.NewOwnerId, 1);

        // Для входящих сообщений ToPeer должен быть fromPeer (отправитель).
        // OwnerPeer = получатель (кому приходит сообщение),
        // ToPeer = отправитель (диалог, в котором появляется сообщение - чат с отправителем).
        // Если OwnerPeer == ToPeer, сообщение попадёт в "Избранное" (чат с самим собой)
        var messageItem = new MessageItem(
            toPeer,   // OwnerPeer - новый владелец (получатель) = 2012001
            fromPeer, // ToPeer - отправитель (диалог с отправителем) = 2010002
            fromPeer, // SenderPeer - покупатель/отправитель = 2010002
            fromUserId,
            inboxMessageId,
            string.Empty,
            currentDate,
            0,
            false,
            SendMessageType.MessageService,
            MessageType.Text,
            MessageSubType.Normal,
            MessageAction: messageAction,
            MessageActionType: MessageActionType.CustomAction,
            Pts: inboxPts
        );
        
        var inboxMessageAggregateId = MessageId.Create(toPeer.PeerId, inboxMessageId);
        
        var createInboxCommand = new CreateInboxMessageCommand(
            inboxMessageAggregateId,
            aggregateEvent.RequestInfo,
            messageItem,
            inboxMessageId
        );
        
        await commandBus.PublishAsync(createInboxCommand, cancellationToken);
        
        var receiveInboxCommand = new ReceiveInboxMessageCommand(
            DialogId.Create(messageItem.OwnerPeer.PeerId, messageItem.ToPeer),
            aggregateEvent.RequestInfo,
            messageItem.MessageId,
            messageItem.OwnerPeer.PeerId,
            messageItem.ToPeer,
            null
        );
        
        await commandBus.PublishAsync(receiveInboxCommand, cancellationToken);

        Console.WriteLine($"[StarGiftTransferredEvent] Inbox message created successfully!");
        Console.WriteLine($"   MessageId: {inboxMessageId}");
        Console.WriteLine($"   Recipient (OwnerPeer): {aggregateEvent.NewOwnerId}");
        Console.WriteLine($"   Sender (SenderPeer): {fromUserId}");
        Console.WriteLine($"   IsResale: {isResale}");
        Console.WriteLine($"   ResaleStars: {resaleStarsValue}");
        Console.WriteLine($"   MessageAction: {messageAction.GetType().Name}");

        logger.LogInformation("Star gift transfer message created: MessageId={MessageId} To={ToUserId}",
            inboxMessageId, aggregateEvent.NewOwnerId);

        // Создаём исходящее сообщение для отправителя (покупателя), чтобы он видел подарок в своём чате.
        // Особенно важно при перепродаже: покупателю нужно видеть, что он отправил подарок
        if (fromUserId != aggregateEvent.NewOwnerId)
        {
            var outboxMessageId = await idGenerator.NextIdAsync(IdType.MessageId, fromUserId, 1);
            var outboxPts = await idGenerator.NextIdAsync(IdType.Pts, fromUserId, 1);

            var outboxFromPeer = new Peer(PeerType.User, fromUserId); // Покупатель/отправитель
            var outboxToPeer = new Peer(PeerType.User, aggregateEvent.NewOwnerId); // Получатель

            var outboxMessageItem = new MessageItem(
                outboxFromPeer, // OwnerPeer - отправитель/покупатель = 2010002
                outboxToPeer,   // ToPeer - получатель = 2012001
                outboxFromPeer, // SenderPeer - отправитель/покупатель = 2010002
                fromUserId,
                outboxMessageId,
                string.Empty,
                currentDate,
                0,
                true, // Out = true для исходящего сообщения
                SendMessageType.MessageService,
                MessageType.Text,
                MessageSubType.Normal,
                MessageAction: messageAction,
                MessageActionType: MessageActionType.CustomAction,
                Pts: outboxPts
            );
            
            var outboxMessageAggregateId = MessageId.Create(outboxFromPeer.PeerId, outboxMessageId);
            
            var createOutboxCommand = new CreateOutboxMessageCommand(
                outboxMessageAggregateId,
                aggregateEvent.RequestInfo,
                outboxMessageItem,
                mentionedUserIds: null,
                replyToMsgItems: null,
                clearDraft: true,
                groupItemCount: 1,
                linkedChannelId: null,
                chatMembers: null
            );
            
            await commandBus.PublishAsync(createOutboxCommand, cancellationToken);
            
            var setOutboxTopMessageCommand = new SetOutboxTopMessageCommand(
                DialogId.Create(outboxMessageItem.OwnerPeer.PeerId, outboxMessageItem.ToPeer),
                outboxMessageItem.MessageId,
                outboxMessageItem.OwnerPeer.PeerId,
                outboxMessageItem.ToPeer,
                false // clearDraft
            );

            await commandBus.PublishAsync(setOutboxTopMessageCommand, cancellationToken);

            Console.WriteLine($"[StarGiftTransferredEvent] Outbox message created for sender!");
            Console.WriteLine($"   OutboxMessageId: {outboxMessageId}");
            Console.WriteLine($"   Sender (OwnerPeer): {fromUserId}");
            Console.WriteLine($"   Recipient (ToPeer): {aggregateEvent.NewOwnerId}");

            logger.LogInformation("Star gift transfer outbox message created: MessageId={MessageId} From={FromUserId}",
                outboxMessageId, fromUserId);
        }
        else
        {
            Console.WriteLine($"[StarGiftTransferredEvent] Skipping outbox creation: sender == recipient (self-gift)");
        }
    }
}
