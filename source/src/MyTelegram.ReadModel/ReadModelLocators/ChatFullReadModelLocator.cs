using MyTelegram.Domain.Aggregates.Chat;
using MyTelegram.Domain.Events.GroupCall;

namespace MyTelegram.ReadModel.ReadModelLocators;

public class ChatFullReadModelLocator : IReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        switch (domainEvent.GetAggregateEvent())
        {
            case GroupCallCreatedEvent groupCallCreatedEvent:
                if (groupCallCreatedEvent.PeerType == PeerType.Chat)
                {
                    yield return ChatId.Create(groupCallCreatedEvent.PeerId).Value;
                }
                break;
            case GroupCallDiscardedEvent groupCallDiscardedEvent:
                if (groupCallDiscardedEvent.PeerType == PeerType.Chat)
                {
                    yield return ChatId.Create(groupCallDiscardedEvent.PeerId).Value;
                }
                break;
            default:
                // For ChatAggregate events, use aggregate ID
                yield return domainEvent.GetIdentity().Value;
                break;
        }
    }
}
