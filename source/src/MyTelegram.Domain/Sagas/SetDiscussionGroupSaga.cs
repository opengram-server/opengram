namespace MyTelegram.Domain.Sagas;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<SetDiscussionGroupSagaId>))]
public class SetDiscussionGroupSagaId(string value) : Identity<SetDiscussionGroupSagaId>(value), ISagaId;

public class
    SetDiscussionGroupSagaLocator : DefaultSagaLocator<SetDiscussionGroupSaga, SetDiscussionGroupSagaId>
{
    protected override SetDiscussionGroupSagaId CreateSagaId(string requestId)
    {
        return new SetDiscussionGroupSagaId(requestId);
    }
}

public class SetDiscussionGroupSagaState : AggregateState<SetDiscussionGroupSaga, SetDiscussionGroupSagaId,
    SetDiscussionGroupSagaState>,
    IApply<SetDiscussionGroupSagaStartedSagaEvent>,
    IApply<DiscussionGroupUpdatedSagaEvent>
{
    public RequestInfo RequestInfo { get; private set; } = default!;

    public void Apply(SetDiscussionGroupSagaStartedSagaEvent aggregateEvent)
    {
        RequestInfo = aggregateEvent.RequestInfo;
    }

    public void Apply(DiscussionGroupUpdatedSagaEvent aggregateEvent)
    {
    }
}

public class SetDiscussionGroupSagaStartedSagaEvent(RequestInfo requestInfo) : AggregateEvent<SetDiscussionGroupSaga, SetDiscussionGroupSagaId>
{
    public RequestInfo RequestInfo { get; } = requestInfo;
}

public class DiscussionGroupUpdatedSagaEvent : AggregateEvent<SetDiscussionGroupSaga, SetDiscussionGroupSagaId>
{

}

public class SetDiscussionGroupSaga :
    MyInMemoryAggregateSaga<SetDiscussionGroupSaga,
        SetDiscussionGroupSagaId, SetDiscussionGroupSagaLocator>,
    ISagaIsStartedBy<TempAggregate, TempId, SetChannelDiscussionGroupStartedEvent>,
    ISagaHandles<ChannelAggregate, ChannelId, DiscussionGroupUpdatedEvent>,
    ISagaHandles<ChannelAggregate, ChannelId, LinkedChannelChangedEvent>
{
    private readonly SetDiscussionGroupSagaState _state = new();

    public SetDiscussionGroupSaga(SetDiscussionGroupSagaId id, IEventStore eventStore, IIdGenerator idGenerator) : base(id, eventStore)
    {
        Register(_state);
    }

    public Task HandleAsync(IDomainEvent<TempAggregate, TempId, SetChannelDiscussionGroupStartedEvent> domainEvent,
        ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        Emit(new SetDiscussionGroupSagaStartedSagaEvent(domainEvent.AggregateEvent.RequestInfo));
        var command = new SetDiscussionGroupCommand(ChannelId.Create(domainEvent.AggregateEvent.BroadcastChannelId),
            domainEvent.AggregateEvent.RequestInfo,
            domainEvent.AggregateEvent.BroadcastChannelId,
            domainEvent.AggregateEvent.DiscussionGroupChannelId);
        Publish(command);

        return Task.CompletedTask;
    }

    public Task HandleAsync(IDomainEvent<ChannelAggregate, ChannelId, DiscussionGroupUpdatedEvent> domainEvent,
        ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        Emit(new DiscussionGroupUpdatedSagaEvent());
        var needUpdateBroadcastChannelOfOldLinkedGroup = false;
        if (domainEvent.AggregateEvent.GroupChannelId.HasValue)
        {
            var command =
                new SetLinkedChannelIdCommand(ChannelId.Create(domainEvent.AggregateEvent.GroupChannelId.Value),
                    _state.RequestInfo,
                    domainEvent.AggregateEvent.BroadcastChannelId);
            Publish(command);
            needUpdateBroadcastChannelOfOldLinkedGroup = true;
        }

        if (domainEvent.AggregateEvent.OldGroupChannelId.HasValue)
        {
            var command =
                new SetLinkedChannelIdCommand(ChannelId.Create(domainEvent.AggregateEvent.OldGroupChannelId.Value),
                    _state.RequestInfo,
                    null);
            Publish(command);
        }

        if (!needUpdateBroadcastChannelOfOldLinkedGroup)
        {
            return CompleteAsync(cancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task HandleAsync(IDomainEvent<ChannelAggregate, ChannelId, LinkedChannelChangedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.OldLinkedChannelId.HasValue)
        {
            var command = new SetLinkedChannelIdCommand(
                ChannelId.Create(domainEvent.AggregateEvent.OldLinkedChannelId.Value),
                _state.RequestInfo, null);
            Publish(command);
        }

        return CompleteAsync(cancellationToken);
    }
}