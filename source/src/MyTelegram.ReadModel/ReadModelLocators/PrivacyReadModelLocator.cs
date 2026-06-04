using MyTelegram.Domain.Events.Privacy;

namespace MyTelegram.ReadModel.ReadModelLocators;

public class PrivacyReadModelLocator : IPrivacyReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case IDomainEvent<UserAggregate, UserId, PrivacyRulesUpdatedEvent> e:
                yield return PrivacyId.Create(e.AggregateEvent.UserId, e.AggregateEvent.PrivacyType);
                break;
        }
    }
}
