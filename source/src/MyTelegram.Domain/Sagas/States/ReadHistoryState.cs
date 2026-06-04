namespace MyTelegram.Domain.Sagas.States;

public class ReadHistoryState : AggregateState<ReadHistorySaga, ReadHistorySagaId, ReadHistoryState>,
        IApply<ReadHistoryStartedSagaEvent>,
        IApply<UpdateInboxMaxIdCompletedSagaEvent>,
        IApply<UpdateOutboxMaxIdCompletedSagaEvent>,
        IApply<ReadHistoryPtsIncrementedSagaEvent>
{
    public RequestInfo RequestInfo { get; private set; } = default!;

    public void Apply(ReadHistoryStartedSagaEvent aggregateEvent)
    {
        RequestInfo = aggregateEvent.RequestInfo;
    }

    public void Apply(UpdateInboxMaxIdCompletedSagaEvent aggregateEvent)
    {

    }

    public void Apply(UpdateOutboxMaxIdCompletedSagaEvent aggregateEvent)
    {
    }

    public void Apply(ReadHistoryPtsIncrementedSagaEvent aggregateEvent)
    {
    }
}
