namespace MyTelegram.Messenger.QueryServer.EventHandlers;

public class UserIsOnlineEventHandler(
    ILogger<UserIsOnlineEventHandler> logger)
    : IEventHandler<UserIsOnlineEvent>
        , ITransientDependency
{
    public Task HandleEventAsync(UserIsOnlineEvent eventData)
    {
        logger.LogInformation("User {UserId} is online, tempAuthKeyId: {TempAuthKeyId:x2}, permAuthKeyId: {PermAuthKeyId:x2}",
            eventData.UserId,
            eventData.TempAuthKeyId,
            eventData.PermAuthKeyId);

        return Task.CompletedTask;
    }
}
