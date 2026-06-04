using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Events.GroupCall;

namespace MyTelegram.Domain.Sagas;

public class GroupCallMessageSagaStartedSagaEvent(
    RequestInfo requestInfo,
    long callId,
    long accessHash,
    long peerId,
    PeerType peerType,
    int date) : RequestAggregateEvent2<GroupCallMessageSaga, GroupCallMessageSagaId>(requestInfo)
{
    public long CallId { get; } = callId;
    public long AccessHash { get; } = accessHash;
    public long PeerId { get; } = peerId;
    public PeerType PeerType { get; } = peerType;
    public int Date { get; } = date;
}

public class GroupCallMessageSagaCompletedSagaEvent : AggregateEvent<GroupCallMessageSaga, GroupCallMessageSagaId>
{
}

public class GroupCallMessageSagaState : AggregateState<GroupCallMessageSaga, GroupCallMessageSagaId, GroupCallMessageSagaState>,
    IApply<GroupCallMessageSagaStartedSagaEvent>,
    IApply<GroupCallMessageSagaCompletedSagaEvent>
{
    public RequestInfo RequestInfo { get; private set; } = null!;
    public long CallId { get; private set; }
    public long AccessHash { get; private set; }
    public long PeerId { get; private set; }
    public PeerType PeerType { get; private set; }
    public int Date { get; private set; }

    public void Apply(GroupCallMessageSagaStartedSagaEvent aggregateEvent)
    {
        RequestInfo = aggregateEvent.RequestInfo;
        CallId = aggregateEvent.CallId;
        AccessHash = aggregateEvent.AccessHash;
        PeerId = aggregateEvent.PeerId;
        PeerType = aggregateEvent.PeerType;
        Date = aggregateEvent.Date;
    }

    public void Apply(GroupCallMessageSagaCompletedSagaEvent aggregateEvent)
    {
    }
}

/// <summary>
/// Saga that handles sending system messages when group calls are created or discarded
/// </summary>
public class GroupCallMessageSaga : MyInMemoryAggregateSaga<GroupCallMessageSaga, GroupCallMessageSagaId, GroupCallMessageSagaLocator>,
    ISagaIsStartedBy<GroupCallAggregate, GroupCallId, GroupCallCreatedEvent>,
    ISagaHandles<GroupCallAggregate, GroupCallId, GroupCallDiscardedEvent>,
    ISagaHandles<GroupCallAggregate, GroupCallId, GroupCallStartedEvent>
{
    private readonly GroupCallMessageSagaState _state = new();

    public GroupCallMessageSaga(GroupCallMessageSagaId id, IEventStore eventStore) : base(id, eventStore)
    {
        Register(_state);
    }

    /// <summary>
    /// Handles group call creation - sends a system message announcing the call started
    /// </summary>
    public Task HandleAsync(
        IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallCreatedEvent> domainEvent,
        ISagaContext sagaContext,
        CancellationToken cancellationToken)
    {
        Emit(new GroupCallMessageSagaStartedSagaEvent(
            domainEvent.AggregateEvent.RequestInfo,
            domainEvent.AggregateEvent.CallId,
            domainEvent.AggregateEvent.AccessHash,
            domainEvent.AggregateEvent.PeerId,
            domainEvent.AggregateEvent.PeerType,
            domainEvent.AggregateEvent.Date));

        // Don't send message for scheduled calls - they will send message when started
        if (domainEvent.AggregateEvent.ScheduleDate.HasValue)
        {
            return Task.CompletedTask;
        }

        SendGroupCallMessage(
            domainEvent.AggregateEvent.RequestInfo,
            domainEvent.AggregateEvent.PeerId,
            domainEvent.AggregateEvent.PeerType,
            domainEvent.AggregateEvent.CallId,
            domainEvent.AggregateEvent.AccessHash,
            domainEvent.AggregateEvent.Date,
            duration: null); // No duration when call starts

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles scheduled group call start - sends a system message announcing the call started
    /// </summary>
    public Task HandleAsync(
        IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallStartedEvent> domainEvent,
        ISagaContext sagaContext,
        CancellationToken cancellationToken)
    {
        SendGroupCallMessage(
            domainEvent.AggregateEvent.RequestInfo,
            _state.PeerId,
            _state.PeerType,
            _state.CallId,
            _state.AccessHash,
            domainEvent.AggregateEvent.Date,
            duration: null); // No duration when call starts

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles group call discard - sends a system message with call duration
    /// </summary>
    public Task HandleAsync(
        IDomainEvent<GroupCallAggregate, GroupCallId, GroupCallDiscardedEvent> domainEvent,
        ISagaContext sagaContext,
        CancellationToken cancellationToken)
    {
        var duration = domainEvent.AggregateEvent.Date - _state.Date;

        SendGroupCallMessage(
            domainEvent.AggregateEvent.RequestInfo,
            _state.PeerId,
            _state.PeerType,
            _state.CallId,
            _state.AccessHash,
            domainEvent.AggregateEvent.Date,
            duration: duration);

        Emit(new GroupCallMessageSagaCompletedSagaEvent());

        return CompleteAsync();
    }

    private void SendGroupCallMessage(
        RequestInfo requestInfo,
        long peerId,
        PeerType peerType,
        long callId,
        long accessHash,
        int date,
        int? duration)
    {
        var ownerPeer = peerType == PeerType.Channel 
            ? peerId.ToChannelPeer() 
            : peerId.ToChatPeer();
        var senderUserId = requestInfo.UserId;
        var senderPeer = senderUserId.ToUserPeer();

        var messageAction = new TMessageActionGroupCall
        {
            Call = new TInputGroupCall
            {
                Id = callId,
                AccessHash = accessHash
            },
            Duration = duration
        };

        var messageItem = new MessageItem(
            ownerPeer,
            ownerPeer,
            senderPeer,
            senderUserId,
            0, // Message ID will be assigned
            string.Empty,
            date,
            Random.Shared.NextInt64(),
            true,
            SendMessageType.MessageService,
            MessageType.Text,
            MessageSubType.Normal,
            MessageAction: messageAction,
            MessageActionType: MessageActionType.GroupCall
        );

        var command = new StartSendMessageCommand(
            TempId.New,
            requestInfo,
            [new SendMessageItem(messageItem)]
        );

        Publish(command);
    }
}
