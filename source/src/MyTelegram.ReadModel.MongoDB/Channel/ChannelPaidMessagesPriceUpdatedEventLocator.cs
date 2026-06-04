using EventFlow.Aggregates;
using EventFlow.ReadStores;
using MyTelegram.Domain.Events.Channel;

namespace MyTelegram.ReadModel.MongoDB.Channel;

public class ChannelPaidMessagesPriceUpdatedEventLocator : IReadModelLocator
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var evt = (ChannelPaidMessagesPriceUpdatedEvent)domainEvent.GetAggregateEvent();
        yield return ChannelId.Create(evt.ChannelId).Value;
    }
}
