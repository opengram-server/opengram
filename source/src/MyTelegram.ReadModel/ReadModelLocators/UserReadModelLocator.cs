namespace MyTelegram.ReadModel.ReadModelLocators;

public class UserReadModelLocator : IUserReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var aggregateEvent = domainEvent.GetAggregateEvent();
        switch (aggregateEvent)
        {
            case MessagePinnedUpdatedEvent messagePinnedUpdatedEvent:
                if (messagePinnedUpdatedEvent.ToPeer.PeerType == PeerType.User)
                {
                    yield return UserId.Create(messagePinnedUpdatedEvent.OwnerPeerId).Value;
                }
                break;
            case OutboxMessagePinnedUpdatedEvent outboxMessagePinnedUpdatedEvent:
                if (outboxMessagePinnedUpdatedEvent.ToPeer.PeerType == PeerType.User)
                {
                    yield return UserId.Create(outboxMessagePinnedUpdatedEvent.OwnerPeerId).Value;
                }

                break;
            case InboxMessagePinnedUpdatedEvent inboxMessagePinnedUpdatedEvent:
                if (inboxMessagePinnedUpdatedEvent.ToPeer.PeerType == PeerType.User)
                {
                    yield return UserId.Create(inboxMessagePinnedUpdatedEvent.OwnerPeerId).Value;
                }

                break;
            default:
                yield return domainEvent.GetIdentity().Value;
                break;
        }
    }
}
