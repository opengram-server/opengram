using EventFlow.Aggregates;
using EventFlow.ReadStores;
using MyTelegram.Domain.Events.Chat;

namespace MyTelegram.ReadModel.MongoDB.Chat;

public class ChatAvailableReactionsChangedEventLocator : IReadModelLocator
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var evt = (ChatAvailableReactionsChangedEvent)domainEvent.GetAggregateEvent();
        yield return ChatId.Create(evt.ChatId).Value;
    }
}
