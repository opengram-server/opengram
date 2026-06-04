namespace MyTelegram.Domain.Sagas;
[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<UpdateMessageReplySagaId>))]
public class UpdateMessageReplySagaId(string value) : Identity<UpdateMessageReplySagaId>(value), ISagaId;

public class
    UpdateMessageReplySagaLocator : DefaultSagaLocator<UpdateMessageReplySaga, UpdateMessageReplySagaId>
{
    protected override UpdateMessageReplySagaId CreateSagaId(string requestId)
    {
        return new UpdateMessageReplySagaId(requestId);
    }
}

public class UpdateMessageReplySagaState : AggregateState<UpdateMessageReplySaga, UpdateMessageReplySagaId,
    UpdateMessageReplySagaState>,
    IApply<UpdateMessageReplySagaStartedSagaEvent>,
    IApply<DiscussionGroupUpdatedSagaEvent>
{
    public RequestInfo RequestInfo { get; private set; } = default!;

    public void Apply(UpdateMessageReplySagaStartedSagaEvent aggregateEvent)
    {
        RequestInfo = aggregateEvent.RequestInfo;
    }

    public void Apply(DiscussionGroupUpdatedSagaEvent aggregateEvent)
    {
    }
}

public class UpdateMessageReplySagaStartedSagaEvent(RequestInfo requestInfo) : AggregateEvent<UpdateMessageReplySaga, UpdateMessageReplySagaId>
{
    public RequestInfo RequestInfo { get; } = requestInfo;
}

public class UpdateMessageReplySaga :
    MyInMemoryAggregateSaga<UpdateMessageReplySaga,
        UpdateMessageReplySagaId, UpdateMessageReplySagaLocator>
{
    private readonly UpdateMessageReplySagaState _state = new();

    public UpdateMessageReplySaga(UpdateMessageReplySagaId id, IEventStore eventStore, IIdGenerator idGenerator) : base(id, eventStore)
    {
        Register(_state);
    }
}
