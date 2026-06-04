namespace MyTelegram.Domain.Sagas.States;

public class ReadChannelHistorySagaState : AggregateState<ReadChannelHistorySaga, ReadChannelHistorySagaId,
        ReadChannelHistorySagaState>,
    IApply<ReadChannelHistoryStartedSagaEvent>,
    IApply<ReadChannelHistoryCompletedSagaEvent>
{
    public RequestInfo RequestInfo { get; set; } = null!;
    public long ChannelId { get; private set; }

    public void Apply(ReadChannelHistoryStartedSagaEvent aggregateEvent)
    {
        RequestInfo = aggregateEvent.RequestInfo;
        ChannelId = aggregateEvent.ChannelId;
    }

    public void Apply(ReadChannelHistoryCompletedSagaEvent aggregateEvent)
    {
    }
}
