namespace MyTelegram.Domain.Sagas;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<UnpinAllMessagesSagaId>))]
public class UnpinAllMessagesSagaId(string value) : SingleValueObject<string>(value), ISagaId;

public class UnPinAllMessagesSagaLocator : DefaultSagaLocator<UnpinAllMessagesSaga, UnpinAllMessagesSagaId>
{
    protected override UnpinAllMessagesSagaId CreateSagaId(string requestId)
    {
        return new UnpinAllMessagesSagaId(requestId);
    }
}

public class
    UnpinAllMessagesSagaState : AggregateState<UnpinAllMessagesSaga, UnpinAllMessagesSagaId, UnpinAllMessagesSagaState>,
    IApply<UnpinAllMessagesStartedSagaEvent>,
    IApply<MessageUnpinnedSagaEvent>,
    IApply<UnpinAllMessagesCompletedSagaEvent>,
    IApply<UnpinAllParticipantMessagesCompletedSagaEvent>
{
    public RequestInfo RequestInfo { get; private set; } = default!;
    public IReadOnlyCollection<SimpleMessageItem> MessageItems { get; private set; } = default!;

    public Peer ToPeer { get; private set; } = default!;

    // key=ownerPeerId
    public Dictionary<long, UnPinnedItem> UnPinnedItems { get; set; } = new();
    public int TotalCount { get; private set; }
    public int UnPinnedCount { get; private set; }

    public void Apply(UnpinAllMessagesStartedSagaEvent aggregateEvent)
    {
        RequestInfo = aggregateEvent.RequestInfo;
        ToPeer = aggregateEvent.ToPeer;
        TotalCount = aggregateEvent.MessageItems.Count;
        MessageItems = aggregateEvent.MessageItems;
    }

    public void Apply(MessageUnpinnedSagaEvent aggregateEvent)
    {
        if (!UnPinnedItems.TryGetValue(aggregateEvent.OwnerPeerId, out var item))
        {
            item = new(aggregateEvent.OwnerPeerId, [], aggregateEvent.Pts);
            UnPinnedItems.TryAdd(aggregateEvent.OwnerPeerId, item);
        }

        item.MessageIds.Add(aggregateEvent.MessageId);
        item.Pts = aggregateEvent.Pts;

        UnPinnedCount++;
    }

    public class UnPinnedItem(long userId, List<int> messageIds, int pts)
    {
        public long UserId { get; set; } = userId;
        public List<int> MessageIds { get; } = messageIds;
        public int Pts { get; set; } = pts;
    }

    public void Apply(UnpinAllMessagesCompletedSagaEvent aggregateEvent)
    {
    }

    public void Apply(UnpinAllParticipantMessagesCompletedSagaEvent aggregateEvent)
    {
    }
}

public class UnpinAllMessagesStartedSagaEvent(
    RequestInfo requestInfo,
    IReadOnlyCollection<SimpleMessageItem> messageItems,
    Peer toPeer)
    : AggregateEvent<UnpinAllMessagesSaga, UnpinAllMessagesSagaId>
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public IReadOnlyCollection<SimpleMessageItem> MessageItems { get; } = messageItems;
    public Peer ToPeer { get; } = toPeer;
}

public class MessageUnpinnedSagaEvent(long ownerPeerId, int messageId, int pts) : AggregateEvent<UnpinAllMessagesSaga, UnpinAllMessagesSagaId>
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public int MessageId { get; } = messageId;
    public int Pts { get; } = pts;
}

public class UnpinAllMessagesSaga : MyInMemoryAggregateSaga<UnpinAllMessagesSaga, UnpinAllMessagesSagaId, UnPinAllMessagesSagaLocator>,
        ISagaIsStartedBy<TempAggregate, TempId, UnpinAllMessagesStartedEvent>,
        ISagaHandles<MessageAggregate, MessageId, MessageUnpinnedEvent>
{
    private readonly UnpinAllMessagesSagaState _state = new();
    private readonly IIdGenerator _idGenerator;

    public UnpinAllMessagesSaga(UnpinAllMessagesSagaId id, IEventStore eventStore, IIdGenerator idGenerator) : base(id, eventStore)
    {
        _idGenerator = idGenerator;
        Register(_state);
    }

    public Task HandleAsync(IDomainEvent<TempAggregate, TempId, UnpinAllMessagesStartedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        Emit(new UnpinAllMessagesStartedSagaEvent(domainEvent.AggregateEvent.RequestInfo, domainEvent.AggregateEvent.MessageItems, domainEvent.AggregateEvent.ToPeer));

        foreach (var item in domainEvent.AggregateEvent.MessageItems)
        {
            var command = new UnpinMessageCommand(MessageId.Create(item.OwnerPeerId, item.MessageId),
                domainEvent.AggregateEvent.RequestInfo);
            Publish(command);
        }

        return Task.CompletedTask;
    }

    public async Task HandleAsync(IDomainEvent<MessageAggregate, MessageId, MessageUnpinnedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        var pts = await _idGenerator.NextIdAsync(IdType.Pts, domainEvent.AggregateEvent.OwnerPeerId, cancellationToken: cancellationToken);
        Emit(new MessageUnpinnedSagaEvent(domainEvent.AggregateEvent.OwnerPeerId, domainEvent.AggregateEvent.MessageId, pts));

        await HandleUnpinnedMessagesCompletedAsync();
    }

    private Task HandleUnpinnedMessagesCompletedAsync()
    {
        if (_state.UnPinnedCount == _state.TotalCount)
        {
            var toPeer = _state.ToPeer;
            if (toPeer.PeerType == PeerType.User)
            {
                toPeer = new Peer(PeerType.User, _state.RequestInfo.UserId);
            }
            foreach (var kv in _state.UnPinnedItems)
            {
                var item = kv.Value;
                if (_state.ToPeer.PeerType == PeerType.Channel || item.UserId == _state.RequestInfo.UserId)
                {
                    var offset = _state.MessageItems.Max(p => p.MessageId);

                    Emit(new UnpinAllMessagesCompletedSagaEvent(_state.RequestInfo, _state.ToPeer, item.Pts, item.MessageIds.Count, offset, item.MessageIds));
                }
                else
                {
                    Emit(new UnpinAllParticipantMessagesCompletedSagaEvent(item.UserId, toPeer, item.Pts, item.MessageIds.Count, item.MessageIds));
                }
            }

            return CompleteAsync();
        }

        return Task.CompletedTask;
    }
}

public class UnpinAllParticipantMessagesCompletedSagaEvent(long ownerPeerId, Peer toPeer, int pts, int ptsCount, List<int> messageIds)
    : AggregateEvent<UnpinAllMessagesSaga, UnpinAllMessagesSagaId>
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public Peer ToPeer { get; } = toPeer;
    public int Pts { get; } = pts;
    public int PtsCount { get; } = ptsCount;
    public List<int> MessageIds { get; } = messageIds;
}

public class UnpinAllMessagesCompletedSagaEvent(RequestInfo requestInfo, Peer toPeer, int pts, int ptsCount, int offset, List<int> messageIds)
    : AggregateEvent<UnpinAllMessagesSaga, UnpinAllMessagesSagaId>
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public Peer ToPeer { get; } = toPeer;
    public int Pts { get; } = pts;
    public int PtsCount { get; } = ptsCount;
    public int Offset { get; } = offset;
    public List<int> MessageIds { get; } = messageIds;
}
