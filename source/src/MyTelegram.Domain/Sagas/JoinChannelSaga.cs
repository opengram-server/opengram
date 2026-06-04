namespace MyTelegram.Domain.Sagas;

public class JoinChannelStartedSagaEvent(RequestInfo requestInfo, long channelId, bool broadcast, int topMessageId, int channelHistoryMinId) : RequestAggregateEvent2<JoinChannelSaga, JoinChannelSagaId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public bool Broadcast { get; } = broadcast;
    public int TopMessageId { get; } = topMessageId;
    public int ChannelHistoryMinId { get; } = channelHistoryMinId;
}

public class ChannelMemberCreatedSagaEvent : AggregateEvent<JoinChannelSaga, JoinChannelSagaId>
{

}

public class JoinChannelCompletedSagaEvent(RequestInfo requestInfo, long channelId, bool broadcast) : RequestAggregateEvent2<JoinChannelSaga, JoinChannelSagaId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public bool Broadcast { get; } = broadcast;
}


public class JoinChannelSagaState : AggregateState<JoinChannelSaga, JoinChannelSagaId, JoinChannelSagaState>,
    IApply<JoinChannelStartedSagaEvent>,
    IApply<ChannelMemberCreatedSagaEvent>,
    IApply<JoinChannelCompletedSagaEvent>
{
    public RequestInfo RequestInfo { get; private set; } = null!;
    public long ChannelId { get; private set; }
    public bool Broadcast { get; private set; }
    public int TopMessageId { get; private set; }
    public int ChannelHistoryMinId { get; private set; }


    public bool IsCompleted { get; private set; } = true;

    public void Apply(JoinChannelStartedSagaEvent aggregateEvent)
    {
        RequestInfo = aggregateEvent.RequestInfo;
        ChannelId = aggregateEvent.ChannelId;
        Broadcast = aggregateEvent.Broadcast;
        TopMessageId = aggregateEvent.TopMessageId;
        ChannelHistoryMinId = aggregateEvent.ChannelHistoryMinId;
    }

    public void Apply(ChannelMemberCreatedSagaEvent aggregateEvent)
    {

    }

    public void Apply(JoinChannelCompletedSagaEvent aggregateEvent)
    {

    }
}

public class JoinChannelSaga : MyInMemoryAggregateSaga<JoinChannelSaga, JoinChannelSagaId, JoinChannelSagaLocator>,
        ISagaIsStartedBy<TempAggregate, TempId, JoinChannelStartedEvent>,
        ISagaHandles<ChannelMemberAggregate, ChannelMemberId, ChannelMemberCreatedEvent>
{
    private readonly JoinChannelSagaState _state = new();
    public JoinChannelSaga(JoinChannelSagaId id, IEventStore eventStore) : base(id, eventStore)
    {
        Register(_state);
    }

    public Task HandleAsync(IDomainEvent<TempAggregate, TempId, JoinChannelStartedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        Emit(new JoinChannelStartedSagaEvent(domainEvent.AggregateEvent.RequestInfo,
            domainEvent.AggregateEvent.ChannelId,
            domainEvent.AggregateEvent.Broadcast,
            domainEvent.AggregateEvent.TopMessageId,
            domainEvent.AggregateEvent.ChannelHistoryMinId));

        var command = new CreateChannelMemberCommand(
            ChannelMemberId.Create(domainEvent.AggregateEvent.ChannelId, domainEvent.AggregateEvent.RequestInfo.UserId),
            domainEvent.AggregateEvent.RequestInfo,
            domainEvent.AggregateEvent.ChannelId,
            domainEvent.AggregateEvent.RequestInfo.UserId,
            domainEvent.AggregateEvent.RequestInfo.UserId,
            DateTime.UtcNow.ToTimestamp(),
            false,
            null,
            domainEvent.AggregateEvent.Broadcast,
            ChatJoinType.BySelf
        );
        Publish(command);

        return Task.CompletedTask;
    }

    public Task HandleAsync(IDomainEvent<ChannelMemberAggregate, ChannelMemberId, ChannelMemberCreatedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        Emit(new ChannelMemberCreatedSagaEvent());
        var command = new IncrementParticipantCountCommand(ChannelId.Create(domainEvent.AggregateEvent.ChannelId));
        Publish(command);

        var toPeer = new Peer(PeerType.Channel, domainEvent.AggregateEvent.ChannelId);
        var createDialogCommand = new CreateDialogCommand(
            DialogId.Create(domainEvent.AggregateEvent.UserId, toPeer),
            domainEvent.AggregateEvent.RequestInfo,
            domainEvent.AggregateEvent.UserId,
            toPeer,
            _state.ChannelHistoryMinId,
            _state.TopMessageId
        );
        Publish(createDialogCommand);

        return HandleJoinChannelCompletedAsync();
    }

    private Task HandleJoinChannelCompletedAsync()
    {
        if (!_state.IsCompleted)
        {
            return Task.CompletedTask;
        }

        if (!_state.Broadcast)
        {
            var ownerPeerId = _state.ChannelId;
            var outMessageId = 0;
            var ownerPeer = ownerPeerId.ToChannelPeer();
            var senderUserId = _state.RequestInfo.UserId;
            var senderPeer = senderUserId.ToUserPeer();

            var messageItem = new MessageItem(
                ownerPeer,
                ownerPeer,
                senderPeer,
                senderUserId,
                outMessageId,
                string.Empty,
                DateTime.UtcNow.ToTimestamp(),
                Random.Shared.NextInt64(),
                true,
                SendMessageType.MessageService,
                MessageType.Text,
                MessageSubType.ChatJoinBySelf,
                MessageAction: new TMessageActionChatAddUser
                {
                    Users = [senderUserId]
                },
                MessageActionType: MessageActionType.ChatAddUser
                );
            var command = new StartSendMessageCommand(TempId.New,
                _state.RequestInfo,
                [new SendMessageItem(messageItem)]
            );
            Publish(command);
        }

        Emit(new JoinChannelCompletedSagaEvent(_state.RequestInfo, _state.ChannelId, _state.Broadcast));

        return CompleteAsync();
    }
}
