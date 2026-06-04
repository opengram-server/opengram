using EventFlow.Aggregates;
using EventFlow.ReadStores;
using MyTelegram.Domain.Events.Channel;

namespace MyTelegram.ReadModel.MongoDB.Channel;

public class ChannelAvailableReactionsChangedEventLocator : IReadModelLocator
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var evt = (ChannelAvailableReactionsChangedEvent)domainEvent.GetAggregateEvent();
        yield return ChannelId.Create(evt.ChannelId).Value;
    }
}
