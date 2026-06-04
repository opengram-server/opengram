using EventFlow.Aggregates;
using EventFlow.ReadStores;
using MyTelegram.Domain.Events.UserPassword;

namespace MyTelegram.ReadModel.MongoDB.UserPassword;

public class PasswordSetEventLocator : IReadModelLocator
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var evt = (PasswordSetEvent)domainEvent.GetAggregateEvent();
        yield return Domain.Aggregates.UserPassword.UserPasswordId.Create(evt.UserId).Value;
    }
}
