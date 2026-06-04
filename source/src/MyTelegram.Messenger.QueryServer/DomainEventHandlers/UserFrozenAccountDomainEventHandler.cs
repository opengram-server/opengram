using MyTelegram.Messenger.Services.Interfaces;

namespace MyTelegram.Messenger.QueryServer.DomainEventHandlers;

/// <summary>
/// Handles domain events related to frozen accounts (freeze/unfreeze)
/// Sends TUpdateUser to notify all contacts when a user is frozen or unfrozen
/// </summary>
public class UserFrozenAccountDomainEventHandler(
    IObjectMessageSender objectMessageSender,
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IAckCacheService ackCacheService,
    IUserConverterService userConverterService,
    IReadModelCacheHelper<IUserReadModel> userReadModelCacheHelper,
    IReadModelCacheHelper<IUserFullReadModel> userFullReadModelCacheHelper,
    ILogger<UserFrozenAccountDomainEventHandler> logger)
    : DomainEventHandlerBase(objectMessageSender, commandBus, idGenerator, ackCacheService),
        ISubscribeSynchronousTo<UserAggregate, UserId, UserAccountFrozenEvent>,
        ISubscribeSynchronousTo<UserAggregate, UserId, UserAccountUnfrozenEvent>
{
    public async Task HandleAsync(
        IDomainEvent<UserAggregate, UserId, UserAccountFrozenEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var userId = domainEvent.AggregateEvent.UserId;
        
        logger.LogInformation(
            "User account frozen: UserId={UserId}, Reason={Reason}, Until={UntilDate}",
            userId,
            domainEvent.AggregateEvent.Reason,
            domainEvent.AggregateEvent.FreezeUntilDate);

        // Invalidate cache to force reload with new frozen state
        userReadModelCacheHelper.Remove(userId);
        userFullReadModelCacheHelper.Remove(userId);

        // Send TUpdateUser to notify all contacts that this user's state changed
        var update = new TUpdateUser
        {
            UserId = userId
        };

        // Load the updated user data (with IsFrozen = true)
        var user = await userConverterService.GetUserAsync(
            domainEvent.AggregateEvent.RequestInfo,
            userId,
            skipSetContactProperties: true,
            skipCheckPrivacy: true,
            layer: domainEvent.AggregateEvent.RequestInfo.Layer);

        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = new TVector<IUser>(user),
            Chats = new TVector<IChat>(),
            Date = DateTime.UtcNow.ToTimestamp()
        };

        // Notify all users who have this user in contacts
        // This will make the frozen account appear with restricted badge and snowflake emoji
        await PushUpdatesToPeerAsync(
            new Peer(PeerType.User, userId),
            updates,
            excludeAuthKeyId: domainEvent.AggregateEvent.RequestInfo.PermAuthKeyId);
    }

    public async Task HandleAsync(
        IDomainEvent<UserAggregate, UserId, UserAccountUnfrozenEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var userId = domainEvent.AggregateEvent.UserId;
        
        logger.LogInformation(
            "User account unfrozen: UserId={UserId}, Reason={Reason}",
            userId,
            domainEvent.AggregateEvent.Reason);

        // Invalidate cache to force reload with IsFrozen = false
        userReadModelCacheHelper.Remove(userId);
        userFullReadModelCacheHelper.Remove(userId);

        // Send TUpdateUser to notify all contacts that this user is no longer frozen
        var update = new TUpdateUser
        {
            UserId = userId
        };

        // Load the updated user data (with IsFrozen = false)
        var user = await userConverterService.GetUserAsync(
            domainEvent.AggregateEvent.RequestInfo,
            userId,
            skipSetContactProperties: true,
            skipCheckPrivacy: true,
            layer: domainEvent.AggregateEvent.RequestInfo.Layer);

        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = new TVector<IUser>(user),
            Chats = new TVector<IChat>(),
            Date = DateTime.UtcNow.ToTimestamp()
        };

        // Notify all users who have this user in contacts
        // This will restore the normal appearance of the account
        await PushUpdatesToPeerAsync(
            new Peer(PeerType.User, userId),
            updates,
            excludeAuthKeyId: domainEvent.AggregateEvent.RequestInfo.PermAuthKeyId);
    }
}
