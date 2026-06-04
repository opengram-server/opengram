using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Events.StarGift;

namespace MyTelegram.ReadModel.ReadModelLocators;

public class StarGiftReadModelLocator : IReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        // ReadModel создаётся только по событию StarGiftSentEvent.
        // В качестве основного идентификатора используем AggregateId — он гарантирует уникальность.
        // Пара ToUserId_MessageId остаётся вторичным индексом для запросов.
        if (domainEvent is IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftSentEvent> sentEvent)
        {
            // Use AggregateId (e.g., "stargift-{guid}") as the primary ID
            // This ensures each gift has a unique ReadModel entry
            yield return sentEvent.AggregateIdentity.Value;
        }
        // For other StarGift events (Save, Upgrade, etc.), they update existing ReadModel
        else if (domainEvent is IDomainEvent<StarGiftAggregate, StarGiftId> starGiftEvent)
        {
            yield return starGiftEvent.AggregateIdentity.Value;
        }
    }
}
