using MyTelegram.ReadModel.ReadModelLocators;

namespace MyTelegram.ReadModel.ReadModelLocators;

public interface IStarsReadModelLocator : IReadModelLocator
{
}

public class StarsReadModelLocator : IStarsReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var aggregateEvent = domainEvent.GetAggregateEvent();
        switch (aggregateEvent)
        {
            case StarsAccountCreatedEvent starsAccountCreatedEvent:
                yield return StarsId.Create(starsAccountCreatedEvent.PeerId).Value;
                break;
            case StarsAddedEvent:
            case StarsSpentEvent:
            case StarsRefundedEvent:
                yield return domainEvent.GetIdentity().Value;
                break;
        }
    }
}
