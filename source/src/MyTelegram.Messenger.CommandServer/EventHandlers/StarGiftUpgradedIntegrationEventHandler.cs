using MongoDB.Bson;
using MongoDB.Driver;
using MyTelegram.Domain.Aggregates.Dialog;
using MyTelegram.Domain.Aggregates.Messaging;
using MyTelegram.Domain.Commands.Dialog;
using MyTelegram.Domain.Commands.Messaging;
using MyTelegram.Schema;
using MyTelegram.Schema.Extensions;

namespace MyTelegram.Messenger.CommandServer.EventHandlers;

public class StarGiftUpgradedIntegrationEventHandler(
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IMongoDatabase mongoDatabase,
    ILogger<StarGiftUpgradedIntegrationEventHandler> logger) : IEventHandler<StarGiftUpgradedIntegrationEvent>
{
    public async Task HandleEventAsync(StarGiftUpgradedIntegrationEvent eventData)
    {
        logger.LogInformation("[StarGiftUpgradedIntegrationEvent] Received! From={FromUserId}, To={ToUserId}, Gift={GiftId}",
            eventData.FromUserId, eventData.ToUserId, eventData.GiftId);

        // Если FromUserId == 0 или FromUserId == ToUserId, это подарок самому себе:
        // сообщения в чате создавать не нужно.
        if (eventData.FromUserId == 0 || eventData.FromUserId == eventData.ToUserId)
        {
            logger.LogInformation("[StarGiftUpgradedIntegrationEvent] Self-gift or anonymous gift, skipping chat message creation");
            return;
        }

        logger.LogInformation("[StarGiftUpgradedIntegrationEvent] Creating upgrade messages: From={FromUserId}, To={ToUserId}, GiftId={GiftId}",
            eventData.FromUserId, eventData.ToUserId, eventData.GiftId);

        // Десериализуем атрибуты подарка
        TVector<IStarGiftAttribute>? attributes = null;
        if (eventData.Attributes.Length > 0)
        {
            try
            {
                attributes = eventData.Attributes.ToArray().ToTObject<TVector<IStarGiftAttribute>>();
                logger.LogInformation("Deserialized {Count} attributes", attributes?.Count ?? 0);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to deserialize attributes");
            }
        }
        
        // Подтягиваем данные о подарке из каталога
        var catalogCollection = mongoDatabase.GetCollection<BsonDocument>("AvailableStarGiftReadModel");
        var catalogFilter = Builders<BsonDocument>.Filter.Eq("GiftId", eventData.GiftId);
        var catalogDoc = await catalogCollection.Find(catalogFilter).FirstOrDefaultAsync();
        
        var title = catalogDoc?.GetValue("Title", $"Gift #{eventData.GiftId}").AsString ?? $"Gift #{eventData.GiftId}";
        var availabilityTotal = catalogDoc?.GetValue("AvailabilityTotal", 1000).ToInt32() ?? 1000;
        var availabilityRemains = catalogDoc?.GetValue("AvailabilityRemains", 0).ToInt32() ?? 0;
        var availabilityIssued = availabilityTotal - availabilityRemains;
        
        // Формируем TStarGiftUnique для action сообщения
        var uniqueIdHash = eventData.AggregateId.GetHashCode();
        var uniqueGiftId = Math.Abs((long)uniqueIdHash) + (eventData.GiftId * 1000000);
        
        var uniqueGift = new TStarGiftUnique
        {
            Id = uniqueGiftId,
            Title = title,
            Slug = string.IsNullOrEmpty(eventData.UniqueSlug) ? $"gift-{eventData.UniqueNum}" : eventData.UniqueSlug,
            Num = eventData.UniqueNum,
            OwnerId = new TPeerUser { UserId = eventData.ToUserId },
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
            FromId = new TPeerUser { UserId = eventData.ToUserId },  // тот, кто улучшил подарок
            Peer = new TPeerUser { UserId = eventData.FromUserId },   // тот, кто подарил
            SavedId = null,
            Gift = uniqueGift
        };
        messageAction.ComputeFlag();

        // Собеседники диалога
        var fromPeer = new Peer(PeerType.User, eventData.FromUserId);  // даритель
        var toPeer = new Peer(PeerType.User, eventData.ToUserId);      // улучшивший подарок

        // Генерируем MessageId для входящего и исходящего сообщений
        var inboxMessageId = await idGenerator.NextIdAsync(IdType.MessageId, eventData.FromUserId);
        var outboxMessageId = await idGenerator.NextIdAsync(IdType.MessageId, eventData.ToUserId);

        // Генерируем Pts
        var gifterPts = await idGenerator.NextIdAsync(IdType.Pts, eventData.FromUserId, 1);
        var upgraderPts = await idGenerator.NextIdAsync(IdType.Pts, eventData.ToUserId, 1);

        logger.LogInformation("Generated MessageIds: Inbox={InboxMessageId}, Outbox={OutboxMessageId}", inboxMessageId, outboxMessageId);

        // Оригинального RequestInfo у нас нет, поэтому собираем минимально необходимый
        var requestInfo = new RequestInfo(
            ReqMsgId: 0,
            UserId: eventData.ToUserId,
            AccessHashKeyId: 0,
            AuthKeyId: 0,
            PermAuthKeyId: 0,
            RequestId: Guid.NewGuid(),
            Layer: 0,
            Date: eventData.UpgradeDate,
            DeviceType: 0,
            AddRequestIdToCache: false,
            IsSubRequest: false
        );
        
        // Входящее сообщение для ДАРИТЕЛЯ: он получает уведомление, что его подарок улучшили
        var inboxMessageItem = new MessageItem(
            fromPeer,  // OwnerPeerId — владелец входящего сообщения, даритель
            toPeer,    // ToPeer — сообщение "от" улучшившего
            toPeer,    // SenderPeerId — отправитель, улучшивший подарок
            eventData.ToUserId,  // SenderUserId
            inboxMessageId,
            string.Empty,
            eventData.UpgradeDate,
            0,
            false,     // для дарителя сообщение входящее
            SendMessageType.MessageService,
            MessageType.Text,
            MessageSubType.Normal,
            MessageAction: messageAction,
            MessageActionType: MessageActionType.CustomAction,
            Pts: gifterPts
        );
        
        var createInboxCommand = new CreateInboxMessageCommand(
            MessageId.Create(fromPeer.PeerId, inboxMessageId),
            requestInfo,
            inboxMessageItem,
            outboxMessageId
        );
        
        await commandBus.PublishAsync(createInboxCommand, CancellationToken.None);
        logger.LogInformation("Inbox message created for gifter (user {FromUserId})", eventData.FromUserId);

        // Обновляем диалог дарителя
        var receiveInboxCommand = new ReceiveInboxMessageCommand(
            DialogId.Create(fromPeer.PeerId, toPeer),
            requestInfo,
            inboxMessageId,
            fromPeer.PeerId,
            toPeer,
            null
        );
        await commandBus.PublishAsync(receiveInboxCommand, CancellationToken.None);
        
        // Исходящее сообщение для УЛУЧШИВШЕГО: он видит у себя в чате, что улучшил
        var outboxMessageItem = new MessageItem(
            toPeer,    // OwnerPeerId — владелец исходящего сообщения, улучшивший
            fromPeer,  // ToPeer — сообщение "для" дарителя
            toPeer,    // SenderPeerId — отправитель, улучшивший подарок
            eventData.ToUserId,  // SenderUserId
            outboxMessageId,
            string.Empty,
            eventData.UpgradeDate,
            0,
            true,      // для улучшившего сообщение исходящее
            SendMessageType.MessageService,
            MessageType.Text,
            MessageSubType.Normal,
            MessageAction: messageAction,
            MessageActionType: MessageActionType.CustomAction,
            Pts: upgraderPts
        );
        
        var createOutboxCommand = new CreateOutboxMessageCommand(
            MessageId.Create(toPeer.PeerId, outboxMessageId),
            requestInfo,
            outboxMessageItem,
            mentionedUserIds: null,
            replyToMsgItems: null,
            clearDraft: false,
            groupItemCount: 1,
            linkedChannelId: null,
            chatMembers: null
        );
        
        await commandBus.PublishAsync(createOutboxCommand, CancellationToken.None);
        logger.LogInformation("Outbox message created for upgrader (user {ToUserId})", eventData.ToUserId);

        // Обновляем диалог улучшившего
        var updateOutboxTopMessageCommand = new SetOutboxTopMessageCommand(
            DialogId.Create(toPeer.PeerId, fromPeer),
            outboxMessageId,
            toPeer.PeerId,
            fromPeer,
            false
        );
        await commandBus.PublishAsync(updateOutboxTopMessageCommand, CancellationToken.None);
        
        logger.LogInformation("[StarGiftUpgradedIntegrationEvent] Chat messages created for upgrade in dialog between {FromUserId} and {ToUserId}",
            eventData.FromUserId, eventData.ToUserId);
    }
}