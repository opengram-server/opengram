using MyTelegram.Domain.Events.Privacy;
using MyTelegram.Messenger.DomainEventHandlers;
using MyTelegram.Messenger.Services.Impl;

namespace MyTelegram.Messenger.CommandServer.DomainEventHandlers;

public class PrivacyEventHandler(
    IObjectMessageSender objectMessageSender,
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IAckCacheService ackCacheService)
    : DomainEventHandlerBase(objectMessageSender, commandBus, idGenerator, ackCacheService),
        ISubscribeSynchronousTo<UserAggregate, UserId, PrivacyRulesUpdatedEvent>
{
    public async Task HandleAsync(IDomainEvent<UserAggregate, UserId, PrivacyRulesUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        
        // Note: PrivacyReadModel persistence is handled automatically by EventFlow
        // via UseMongoDbReadModel<PrivacyReadModel, IPrivacyReadModelLocator>() registration
        
        // Convert privacy rules to schema
        var rules = evt.Rules
            .Select(PrivacyConverter.ToPrivacyRule)
            .ToList();

        var privacyKey = PrivacyConverter.ToPrivacyKey(evt.PrivacyType);

        var update = new TUpdatePrivacy
        {
            Key = privacyKey,
            Rules = new TVector<IPrivacyRule>(rules)
        };

        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Seq = 0
        };

        // Send update to all user's devices
        await PushUpdatesToPeerAsync(
            new Peer(PeerType.User, evt.UserId),
            updates,
            updatesType: UpdatesType.Updates);
    }
}
