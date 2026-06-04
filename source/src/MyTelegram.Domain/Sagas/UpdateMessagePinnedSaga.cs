namespace MyTelegram.Domain.Sagas;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<UpdateMessagePinnedSagaId>))]
public class UpdateMessagePinnedSagaId(string value) : SingleValueObject<string>(value), ISagaId;

public class UpdateMessagePinnedSagaLocator : DefaultSagaLocator<UpdateMessagePinnedSaga, UpdateMessagePinnedSagaId>
{
    protected override UpdateMessagePinnedSagaId CreateSagaId(string requestId)
    {
        return new UpdateMessagePinnedSagaId(requestId);
    }
}

public class
    UpdateMessagePinnedSagaState : AggregateState<UpdateMessagePinnedSaga, UpdateMessagePinnedSagaId,
        UpdateMessagePinnedSagaState>,
    IApply<UpdateMessagePinnedStartedSagaEvent>,
    IApply<MessagePinnedUpdatedSagaEvent>,
    IApply<UpdateMessagePinnedCompletedSagaEvent>
{
    public RequestInfo RequestInfo { get; private set; } = default!;
    public IReadOnlyCollection<SimpleMessageItem> MessageItems { get; private set; } = default!;

    public Peer ToPeer { get; private set; } = default!;
    public bool Pinned { get; private set; }

    public bool PmOneSide { get; private set; }

    // key=ownerPeerId
    public Dictionary<long, PinnedItem> UpdatedItems { get; set; } = new();
    public int TotalCount { get; private set; }
    public int UpdatedCount { get; private set; }

    public void Apply(MessagePinnedUpdatedSagaEvent aggregateEvent)
    {
        if (!UpdatedItems.TryGetValue(aggregateEvent.OwnerPeerId, out var item))
        {
            item = new PinnedItem(aggregateEvent.OwnerPeerId, [], aggregateEvent.Pts);
            UpdatedItems.TryAdd(aggregateEvent.OwnerPeerId, item);
        }

        item.MessageIds.Add(aggregateEvent.MessageId);
        item.Pts = aggregateEvent.Pts;

        UpdatedCount++;
    }

    public void Apply(UpdateMessagePinnedCompletedSagaEvent aggregateEvent)
    {
    }

    public void Apply(UpdateMessagePinnedStartedSagaEvent aggregateEvent)
    {
        RequestInfo = aggregateEvent.RequestInfo;
        ToPeer = aggregateEvent.ToPeer;
        Pinned = aggregateEvent.Pinned;
        MessageItems = aggregateEvent.MessageItems;
        PmOneSide = aggregateEvent.PmOneSide;

        TotalCount = aggregateEvent.MessageItems.Count;
    }

    public void Apply(UpdateParticipantMessagePinnedCompletedSagaEvent aggregateEvent)
    {
    }

    public class PinnedItem(long userId, List<int> messageIds, int pts)
    {
        public long UserId { get; set; } = userId;
        public List<int> MessageIds { get; } = messageIds;
        public int Pts { get; set; } = pts;
    }
}

public class UpdateMessagePinnedStartedSagaEvent(
    RequestInfo requestInfo,
    IReadOnlyCollection<SimpleMessageItem> messageItems,
    Peer toPeer,
    bool pinned,
    bool pmOneSide
)
    : AggregateEvent<UpdateMessagePinnedSaga, UpdateMessagePinnedSagaId>
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public IReadOnlyCollection<SimpleMessageItem> MessageItems { get; } = messageItems;
    public Peer ToPeer { get; } = toPeer;
    public bool Pinned { get; } = pinned;
    public bool PmOneSide { get; } = pmOneSide;
}

public class MessagePinnedUpdatedSagaEvent(long ownerPeerId, int messageId, int pts)
    : AggregateEvent<UpdateMessagePinnedSaga, UpdateMessagePinnedSagaId>
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public int MessageId { get; } = messageId;
    public int Pts { get; } = pts;
}

public class UpdateMessagePinnedSaga : MyInMemoryAggregateSaga<UpdateMessagePinnedSaga, UpdateMessagePinnedSagaId,
        UpdateMessagePinnedSagaLocator>,
    ISagaIsStartedBy<TempAggregate, TempId, UpdateMessagePinnedStartedEvent>,
    ISagaHandles<MessageAggregate, MessageId, MessagePinnedUpdatedEvent>,
    IApply<UpdateParticipantMessagePinnedCompletedSagaEvent>
{
    private readonly IIdGenerator _idGenerator;
    private readonly UpdateMessagePinnedSagaState _state = new();

    public UpdateMessagePinnedSaga(UpdateMessagePinnedSagaId id, IEventStore eventStore, IIdGenerator idGenerator) :
        base(id, eventStore)
    {
        _idGenerator = idGenerator;
        Register(_state);
    }

    public async Task HandleAsync(IDomainEvent<MessageAggregate, MessageId, MessagePinnedUpdatedEvent> domainEvent,
        ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        var pts = await _idGenerator.NextIdAsync(IdType.Pts, domainEvent.AggregateEvent.OwnerPeerId,
            cancellationToken: cancellationToken);
        Emit(new MessagePinnedUpdatedSagaEvent(domainEvent.AggregateEvent.OwnerPeerId,
            domainEvent.AggregateEvent.MessageId, pts));

        await HandleUpdateMessagePinnedCompletedAsync(domainEvent.AggregateEvent.Post);
    }

    public Task HandleAsync(IDomainEvent<TempAggregate, TempId, UpdateMessagePinnedStartedEvent> domainEvent,
        ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        Emit(new UpdateMessagePinnedStartedSagaEvent(domainEvent.AggregateEvent.RequestInfo,
            domainEvent.AggregateEvent.MessageItems, domainEvent.AggregateEvent.ToPeer,
            domainEvent.AggregateEvent.Pinned, domainEvent.AggregateEvent.PmOneSide));

        foreach (var item in domainEvent.AggregateEvent.MessageItems)
        {
            var command = new UpdateMessagePinnedCommand(MessageId.Create(item.OwnerPeerId, item.MessageId),
                domainEvent.AggregateEvent.RequestInfo,
                domainEvent.AggregateEvent.Pinned
            );
            Publish(command);
        }

        return Task.CompletedTask;
    }

    private async Task HandleUpdateMessagePinnedCompletedAsync(bool post)
    {
        if (_state.UpdatedCount == _state.TotalCount)
        {
            var toPeer = _state.ToPeer;
            if (toPeer.PeerType == PeerType.User)
            {
                toPeer = new Peer(PeerType.User, _state.RequestInfo.UserId);
            }

            //var serviceMessageHasSent = false;

            foreach (var kv in _state.UpdatedItems)
            {
                var item = kv.Value;
                if (_state.ToPeer.PeerType == PeerType.Channel || item.UserId == _state.RequestInfo.UserId)
                {
                    var offset = _state.MessageItems.Min(p => p.MessageId);

                    Emit(new UpdateMessagePinnedCompletedSagaEvent(_state.RequestInfo, _state.ToPeer, item.Pts,
                        item.MessageIds.Count, offset, item.MessageIds, _state.Pinned));
                }
                else
                {
                    Emit(new UpdateParticipantMessagePinnedCompletedSagaEvent(item.UserId, toPeer, item.Pts,
                        item.MessageIds.Count, item.MessageIds, _state.Pinned));
                }

                //if (!serviceMessageHasSent&& _state is { Pinned: true, PmOneSide: false })
                //{
                //    // if pinned=true messageIds.Count==1
                //    ReplyToMsgItem? replyToMsgItem = null;
                //    if (_state.ToPeer.PeerType != PeerType.Channel)
                //    {
                //        if (_state.UpdatedItems.TryGetValue(_state.ToPeer.PeerId, out var pinnedItem))
                //        {
                //            replyToMsgItem = new(pinnedItem.UserId, pinnedItem.MessageIds.First());
                //        }
                //    }

                //    await SendServiceMessageToTargetPeerAsync(item.MessageIds.First(), replyToMsgItem);
                //    serviceMessageHasSent = true;
                //}
            }

            if (_state is { Pinned: true, PmOneSide: false })
            {
                // if pinned=true messageIds.Count==1
                ReplyToMsgItem? replyToMsgItem = null;
                var userIdOrChannelId = _state.RequestInfo.UserId;

                if (_state.ToPeer.PeerType != PeerType.Channel)
                {
                    if (_state.UpdatedItems.TryGetValue(_state.ToPeer.PeerId, out var pinnedItem))
                    {
                        replyToMsgItem = new(pinnedItem.UserId, pinnedItem.MessageIds.First());
                    }
                }
                else
                {
                    userIdOrChannelId = _state.ToPeer.PeerId;
                }

                if (_state.UpdatedItems.TryGetValue(userIdOrChannelId, out var item))
                {
                    if (_state.ToPeer.PeerId == _state.RequestInfo.UserId && item.UserId == _state.RequestInfo.UserId)
                    {
                        return;
                    }
                    await SendServiceMessageToTargetPeerAsync(item.MessageIds.First(), replyToMsgItem, post);
                }
            }

            await CompleteAsync();
        }
    }

    public void Apply(UpdateParticipantMessagePinnedCompletedSagaEvent aggregateEvent)
    {
        if (_state is { Pinned: true, PmOneSide: false })
        {

        }
    }

    private async Task SendServiceMessageToTargetPeerAsync(int replyToMsgId, ReplyToMsgItem? replyToMsgItem, bool post)
    {
        var ownerPeerId = _state.RequestInfo.UserId;
        if (_state.ToPeer.PeerType == PeerType.Channel)
        {
            ownerPeerId = _state.ToPeer.PeerId;
        }

        List<ReplyToMsgItem>? replyToMsgItems = null;
        if (replyToMsgItem != null)
        {
            replyToMsgItems = [replyToMsgItem];
        }

        var outMessageId = await _idGenerator.NextIdAsync(IdType.MessageId, ownerPeerId);

        var messageItem = new MessageItem(_state.ToPeer with { PeerId = ownerPeerId },
            _state.ToPeer,
            new Peer(PeerType.User, _state.RequestInfo.UserId),
            _state.RequestInfo.UserId,
            outMessageId,
            string.Empty,
            DateTime.UtcNow.ToTimestamp(),
            Random.Shared.NextInt64(),
            true,
            SendMessageType.MessageService,
            MessageType.Text,
            MessageSubType.UpdatePinnedMessage,
            MessageAction: new TMessageActionPinMessage(),
            InputReplyTo: new TInputReplyToMessage
            {
                ReplyToMsgId = replyToMsgId
            },
            MessageActionType: MessageActionType.PinMessage,
            Post: post,
            ReplyToMsgItems: replyToMsgItems
        );
        var command = new StartSendMessageCommand(TempId.New,
            _state.RequestInfo with { RequestId = Guid.NewGuid() },
            [new SendMessageItem(messageItem)]);

        Publish(command);
    }


}

public class UpdateParticipantMessagePinnedCompletedSagaEvent(
    long ownerPeerId,
    Peer toPeer,
    int pts,
    int ptsCount,
    List<int> messageIds,
    bool pinned)
    : AggregateEvent<UpdateMessagePinnedSaga, UpdateMessagePinnedSagaId>
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public Peer ToPeer { get; } = toPeer;
    public int Pts { get; } = pts;
    public int PtsCount { get; } = ptsCount;
    public List<int> MessageIds { get; } = messageIds;
    public bool Pinned { get; } = pinned;
}

public class UpdateMessagePinnedCompletedSagaEvent(
    RequestInfo requestInfo,
    Peer toPeer,
    int pts,
    int ptsCount,
    int offset,
    List<int> messageIds,
    bool pinned)
    : AggregateEvent<UpdateMessagePinnedSaga, UpdateMessagePinnedSagaId>
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public Peer ToPeer { get; } = toPeer;
    public int Pts { get; } = pts;
    public int PtsCount { get; } = ptsCount;
    public int Offset { get; } = offset;
    public List<int> MessageIds { get; } = messageIds;
    public bool Pinned { get; } = pinned;
}