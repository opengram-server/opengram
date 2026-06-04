using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Commands.Updates;
using MyTelegram.Messenger.Services.Interfaces;
using MyTelegram.Queries;
using MyTelegram.Schema;
using MyTelegram.Services.Services;
using MyTelegram.Domain.Aggregates.Updates;

namespace MyTelegram.Messenger.Services.Impl;

public class ServiceNotificationAppService : BaseAppService, IServiceNotificationAppService
{
    private readonly IObjectMessageSender _objectMessageSender;
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILogger<ServiceNotificationAppService> _logger;
    private readonly ICommandBus _commandBus;
    private readonly IIdGenerator _idGenerator;

    public ServiceNotificationAppService(
        IObjectMessageSender objectMessageSender,
        IQueryProcessor queryProcessor,
        ILogger<ServiceNotificationAppService> logger,
        ICommandBus commandBus,
        IIdGenerator idGenerator)
    {
        _objectMessageSender = objectMessageSender;
        _queryProcessor = queryProcessor;
        _logger = logger;
        _commandBus = commandBus;
        _idGenerator = idGenerator;
    }

    public async Task SendServiceNotificationAsync(
        long userId,
        string type,
        string message,
        bool popup,
        IMessageMedia? media = null,
        List<IMessageEntity>? entities = null,
        bool invertMedia = false)
    {
        _logger.LogInformation("Sending service notification to user {UserId}: Type={Type}, Popup={Popup}",
            userId, type, popup);

        var update = CreateServiceNotificationUpdate(type, message, popup, media, entities, invertMedia);

        // Сохраняем update в базу, чтобы пользователь получил его при подключении
        var globalSeqNo = await _idGenerator.NextLongIdAsync(IdType.GlobalSeqNo);
        
        var command = new CreateUpdatesCommand(
            UpdatesId.New,
            userId, // ownerPeerId
            null, // excludeAuthKeyId
            null, // excludeUserId
            null, // onlySendToUserId
            null, // onlySendToThisAuthKeyId
            UpdatesType.Updates,
            0, // pts
            null, // messageId
            CurrentDate,
            globalSeqNo,
            new[] { update },
            null, // userIds
            null  // chatIds
        );
        
        await _commandBus.PublishAsync(command, CancellationToken.None);

        _logger.LogInformation("Service notification saved for user {UserId}", userId);
    }

    public async Task SendServiceNotificationToUsersAsync(
        List<long> userIds,
        string type,
        string message,
        bool popup,
        IMessageMedia? media = null,
        List<IMessageEntity>? entities = null,
        bool invertMedia = false)
    {
        _logger.LogInformation("Sending service notification to {Count} users: Type={Type}, Popup={Popup}",
            userIds.Count, type, popup);

        var update = CreateServiceNotificationUpdate(type, message, popup, media, entities, invertMedia);

        foreach (var userId in userIds)
        {
            try
            {
                var globalSeqNo = await _idGenerator.NextLongIdAsync(IdType.GlobalSeqNo);
                
                var command = new CreateUpdatesCommand(
                    UpdatesId.New,
                    userId, // ownerPeerId
                    null, // excludeAuthKeyId
                    null, // excludeUserId
                    null, // onlySendToUserId
                    null, // onlySendToThisAuthKeyId
                    UpdatesType.Updates,
                    0, // pts
                    null, // messageId
                    CurrentDate,
                    globalSeqNo,
                    new[] { update },
                    null, // userIds
                    null  // chatIds
                );
                
                await _commandBus.PublishAsync(command, CancellationToken.None);

                _logger.LogDebug("Service notification saved for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send service notification to user {UserId}", userId);
            }
        }

        _logger.LogInformation("Service notification sent to {Count} users", userIds.Count);
    }

    public async Task SendServiceNotificationToAllUsersAsync(
        string type,
        string message,
        bool popup,
        IMessageMedia? media = null,
        List<IMessageEntity>? entities = null,
        bool invertMedia = false)
    {
        _logger.LogInformation("Sending service notification to ALL users: Type={Type}, Popup={Popup}",
            type, popup);

        // Получаем идентификаторы всех пользователей из базы
        var allUsers = await _queryProcessor.ProcessAsync(new GetAllUserIdsQuery(), CancellationToken.None);

        if (allUsers == null || !allUsers.Any())
        {
            _logger.LogWarning("No users found to send notification");
            return;
        }

        await SendServiceNotificationToUsersAsync(
            allUsers.ToList(),
            type,
            message,
            popup,
            media,
            entities,
            invertMedia);
    }

    private TUpdateServiceNotification CreateServiceNotificationUpdate(
        string type,
        string message,
        bool popup,
        IMessageMedia? media,
        List<IMessageEntity>? entities,
        bool invertMedia)
    {
        return new TUpdateServiceNotification
        {
            Type = type,
            Message = message,
            Popup = popup,
            InboxDate = CurrentDate,
            Media = media ?? new TMessageMediaEmpty(),
            Entities = entities != null ? new TVector<IMessageEntity>(entities) : new TVector<IMessageEntity>(),
            InvertMedia = invertMedia
        };
    }

}
