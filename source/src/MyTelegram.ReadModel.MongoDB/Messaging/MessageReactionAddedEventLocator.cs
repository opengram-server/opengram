using EventFlow.Aggregates;
using EventFlow.ReadStores;
using MyTelegram.Domain.Events.Messaging;

namespace MyTelegram.ReadModel.MongoDB.Messaging;

public class MessageReactionAddedEventLocator : IReadModelLocator
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var evt = (MessageReactionAddedEvent)domainEvent.GetAggregateEvent();
        yield return MessageId.Create(evt.OwnerPeerId, evt.MessageId, false).Value;
    }
}
