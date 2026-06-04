using MongoDB.Driver;
using EventFlow.Exceptions;
using MyTelegram.Domain.Shared.Events;
using MyTelegram.Domain.Commands.StarGift;
using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.Stars;
using MyTelegram.Domain.Aggregates.Stars;
using EventFlow.Commands;
using MyTelegram.ReadModel.Impl;
using MyTelegram.Schema;

namespace MyTelegram.Messenger.CommandServer.EventHandlers;

/// <summary>
/// Обрабатывает BotGiftEvent от Bot API и запускает отправку подарка за звёзды.
/// </summary>
public class BotGiftEventHandler(
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IMongoDatabase mongoDatabase,
    ILogger<BotGiftEventHandler> logger)
    : IEventHandler<BotGiftEvent>, ITransientDependency
{
    public async Task HandleEventAsync(BotGiftEvent eventData)
    {
        logger.LogInformation("Received BotGiftEvent - BotId={BotId}, ToUserId={ToUserId}, GiftId={GiftId}, Count={Count}",
            eventData.BotUserId, eventData.ToUserId, eventData.GiftId, eventData.Count);

        var giftsCollection = mongoDatabase.GetCollection<AvailableStarGiftReadModel>("AvailableStarGiftReadModel");
        var gift = await giftsCollection.Find(g => g.GiftId == eventData.GiftId).FirstOrDefaultAsync();
        if (gift == null)
        {
            logger.LogWarning("Gift {GiftId} not found in catalog", eventData.GiftId);
            return;
        }

        if (gift.SoldOut)
        {
            logger.LogWarning("Gift {GiftId} is sold out", eventData.GiftId);
            return;
        }

        // Убеждаемся, что у отправителя (бот или админ) есть счёт звёзд
        try
        {
            await commandBus.PublishAsync(
                new CreateStarsAccountCommand(
                    StarsId.Create(eventData.BotUserId),
                    new CommandId($"command-{Guid.NewGuid()}"),
                    eventData.BotUserId),
                CancellationToken.None);
        }
        catch (DomainError)
        {
            // Счёт уже существует
        }

        var requestInfo = new RequestInfo(
            ReqMsgId: eventData.RandomId,
            UserId: eventData.BotUserId,
            AccessHashKeyId: 0,
            AuthKeyId: 0,
            PermAuthKeyId: 0,
            RequestId: Guid.NewGuid(),
            Layer: 0,
            Date: eventData.Timestamp,
            DeviceType: DeviceType.Android,
            AddRequestIdToCache: false,
            IsSubRequest: false
        );

        var canUpgrade = gift.UpgradeStars.HasValue && !eventData.IncludeUpgrade;
        var giftSticker = gift.Sticker.HasValue ? BitConverter.GetBytes(gift.Sticker.Value) : null;
        var count = Math.Max(1, eventData.Count);

        for (var i = 0; i < count; i++)
        {
            var messageId = await idGenerator.NextIdAsync(IdType.MessageId, eventData.ToUserId);
            var aggregateId = StarGiftId.New;

            var command = new InitiateStarGiftCommand(
                aggregateId,
                requestInfo,
                gift.GiftId,
                eventData.BotUserId,
                eventData.ToUserId,
                null,
                messageId,
                gift.Stars,
                gift.ConvertStars,
                eventData.Message,
                eventData.HideName,
                canUpgrade,
                gift.UpgradeStars,
                giftSticker,
                (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            );

            await commandBus.PublishAsync(command, CancellationToken.None);
            logger.LogInformation("Initiated bot gift {GiftId} (AggregateId={AggregateId}) to {ToUserId}",
                gift.GiftId, aggregateId.Value, eventData.ToUserId);
        }
    }
}
