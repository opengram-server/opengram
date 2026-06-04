namespace MyTelegram.Domain.Sagas;


[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<PinForwardedChannelMessageSagaId>))]
public class PinForwardedChannelMessageSagaId(string value) : Identity<PinForwardedChannelMessageSagaId>(value), ISagaId;

public class
    PinChannelMessagePtsIncrementedSagaEvent(long channelId, int messageId, int pts) : AggregateEvent<PinForwardedChannelMessageSaga,
        PinForwardedChannelMessageSagaId>
{
    public long ChannelId { get; private set; } = channelId;
    public int MessageId { get; } = messageId;
    public int Pts { get; private set; } = pts;
}

public class
    PinForwardedChannelMessageSagaLocator : DefaultSagaLocator<PinForwardedChannelMessageSaga, PinForwardedChannelMessageSagaId>
{
    protected override PinForwardedChannelMessageSagaId CreateSagaId(string requestId)
    {
        return new PinForwardedChannelMessageSagaId(requestId);
    }
}

public class PinForwardedChannelMessageSaga(PinForwardedChannelMessageSagaId id, IEventStore eventStore, IIdGenerator idGenerator) : MyInMemoryAggregateSaga<PinForwardedChannelMessageSaga,
    PinForwardedChannelMessageSagaId, PinForwardedChannelMessageSagaLocator>(id, eventStore),
    ISagaIsStartedBy<MessageAggregate, MessageId, ChannelMessagePinnedEvent>,
    IApply<PinChannelMessagePtsIncrementedSagaEvent>
{
    public async Task HandleAsync(IDomainEvent<MessageAggregate, MessageId, ChannelMessagePinnedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        await IncrementPtsAsync(domainEvent.AggregateEvent.ChannelId, domainEvent.AggregateEvent.MessageId);
    }

    private async Task IncrementPtsAsync(long channelId, int messageId)
    {
        var pts = await idGenerator.NextIdAsync(IdType.Pts, channelId);
        Emit(new PinChannelMessagePtsIncrementedSagaEvent(channelId, messageId, pts));
    }

    public void Apply(PinChannelMessagePtsIncrementedSagaEvent aggregateEvent)
    {
        CompleteAsync();
    }
}