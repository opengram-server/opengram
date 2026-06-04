namespace MyTelegram.Domain.Sagas.Events;

public class ReadChannelHistoryStartedSagaEvent(RequestInfo requestInfo, long channelId)
    : RequestAggregateEvent2<ReadChannelHistorySaga, ReadChannelHistorySagaId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
}
