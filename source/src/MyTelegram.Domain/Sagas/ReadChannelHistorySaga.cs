namespace MyTelegram.Domain.Sagas;

public class ReadChannelHistorySaga : MyInMemoryAggregateSaga<ReadChannelHistorySaga, ReadChannelHistorySagaId,
        ReadChannelHistorySagaLocator>,
    ISagaIsStartedBy<DialogAggregate, DialogId, UpdateReadChannelInboxEvent>,
    ISagaHandles<DialogAggregate, DialogId, UpdateReadChannelOutboxEvent>
{
    private readonly ReadChannelHistorySagaState _state = new();

    public ReadChannelHistorySaga(ReadChannelHistorySagaId id
        , IEventStore eventStore) : base(id, eventStore)
    {
        Register(_state);
    }

    public Task HandleAsync(IDomainEvent<DialogAggregate, DialogId, UpdateReadChannelOutboxEvent> domainEvent,
        ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        Emit(new ReadChannelHistoryCompletedSagaEvent(domainEvent.AggregateEvent.RequestInfo,
            domainEvent.AggregateEvent.ChannelId, domainEvent.AggregateEvent.MessageSenderUserId,
            domainEvent.AggregateEvent.MaxId));
        return CompleteAsync(cancellationToken);
    }

    public Task HandleAsync(IDomainEvent<DialogAggregate, DialogId, UpdateReadChannelInboxEvent> domainEvent,
        ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        Emit(new ReadChannelHistoryStartedSagaEvent(domainEvent.AggregateEvent.RequestInfo,
            domainEvent.AggregateEvent.ChannelId));

        var command = new UpdateReadChannelOutboxCommand(
            DialogId.Create(domainEvent.AggregateEvent.MessageSenderUserId, PeerType.Channel,
                domainEvent.AggregateEvent.ChannelId),
            _state.RequestInfo,
            domainEvent.AggregateEvent.MaxId);
        Publish(command);

        CreateReadingHistory(domainEvent.AggregateEvent.ChannelId, domainEvent.AggregateEvent.MaxId);

        return Task.CompletedTask;
    }

    private void CreateReadingHistory(long channelId, int messageId)
    {
        var command = new CreateReadingHistoryCommand(
            ReadingHistoryId.Create(_state.RequestInfo.UserId, channelId, messageId), _state.RequestInfo.UserId,
            channelId,
            messageId, DateTime.UtcNow.ToTimestamp());
        Publish(command);
    }
}