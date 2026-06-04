using EventFlow.Aggregates;
using EventFlow.ReadStores;
using MyTelegram.Domain.Events.UserPassword;

namespace MyTelegram.ReadModel.MongoDB.UserPassword;

public class SrpSessionCreatedEventLocator : IReadModelLocator
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var evt = (SrpSessionCreatedEvent)domainEvent.GetAggregateEvent();
        yield return Domain.Aggregates.UserPassword.UserPasswordId.Create(evt.UserId).Value;
    }
}
