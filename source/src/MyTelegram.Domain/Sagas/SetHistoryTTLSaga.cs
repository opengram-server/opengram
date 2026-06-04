namespace MyTelegram.Domain.Sagas;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<SetHistoryTTLSagaId>))]
public class SetHistoryTTLSagaId(string value) : SingleValueObject<string>(value), ISagaId;

public class SetHistoryTTLSagaLocator : DefaultSagaLocator<SetHistoryTTLSaga, SetHistoryTTLSagaId>
{
    protected override SetHistoryTTLSagaId CreateSagaId(string requestId)
    {
        return new SetHistoryTTLSagaId(requestId);
    }
}

/// <summary>
/// Saga to send service message when history TTL is changed
/// </summary>
public class SetHistoryTTLSaga(SetHistoryTTLSagaId id, IEventStore eventStore, IIdGenerator idGenerator)
    : MyInMemoryAggregateSaga<SetHistoryTTLSaga, SetHistoryTTLSagaId, SetHistoryTTLSagaLocator>(id, eventStore),
        ISagaIsStartedBy<DialogAggregate, DialogId, DialogHistoryTTLUpdatedEvent>,
        ISagaIsStartedBy<ChannelAggregate, ChannelId, ChannelHistoryTTLUpdatedEvent>
{
    public async Task HandleAsync(
        IDomainEvent<DialogAggregate, DialogId, DialogHistoryTTLUpdatedEvent> domainEvent,
        ISagaContext sagaContext,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        
        // Only send service message if TTL is actually set (not null and not 0)
        if (evt.TtlPeriod == null || evt.TtlPeriod == 0)
        {
            await CompleteAsync(cancellationToken);
            return;
        }
        
        // Generate message ID for the service message
        var messageId = await idGenerator.NextIdAsync(IdType.MessageId, evt.OwnerPeerId, cancellationToken: cancellationToken);
        
        var ownerPeer = new Peer(PeerType.User, evt.OwnerPeerId);
        var senderPeer = new Peer(PeerType.User, evt.RequestInfo.UserId);
        
        // Create service message with SetMessagesTTL action
        var messageItem = new MessageItem(
            ownerPeer,
            evt.Peer,
            senderPeer,
            evt.RequestInfo.UserId,
            messageId,
            string.Empty,
            DateTime.UtcNow.ToTimestamp(),
            Random.Shared.NextInt64(),
            true,
            SendMessageType.MessageService,
            MessageType.Text,
            MessageSubType.Normal,
            null,
            new TMessageActionSetMessagesTTL
            {
                Period = evt.TtlPeriod ?? 0
            },
            MessageActionType.SetMessagesTtl
        );
        
        var command = new StartSendMessageCommand(
            TempId.New,
            evt.RequestInfo,
            [new SendMessageItem(messageItem)]
        );
        
        Publish(command);
        await CompleteAsync(cancellationToken);
    }

    public async Task HandleAsync(
        IDomainEvent<ChannelAggregate, ChannelId, ChannelHistoryTTLUpdatedEvent> domainEvent,
        ISagaContext sagaContext,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        
        // Only send service message if TTL is actually set (not null and not 0)
        if (evt.TtlPeriod == null || evt.TtlPeriod == 0)
        {
            await CompleteAsync(cancellationToken);
            return;
        }
        
        // Generate message ID for the service message
        var messageId = await idGenerator.NextIdAsync(IdType.MessageId, evt.ChannelId, cancellationToken: cancellationToken);
        
        var channelPeer = new Peer(PeerType.Channel, evt.ChannelId);
        var senderPeer = new Peer(PeerType.User, evt.RequestInfo.UserId);
        
        // Create service message with SetMessagesTTL action
        var messageItem = new MessageItem(
            channelPeer,
            channelPeer,
            senderPeer,
            evt.RequestInfo.UserId,
            messageId,
            string.Empty,
            DateTime.UtcNow.ToTimestamp(),
            Random.Shared.NextInt64(),
            true,
            SendMessageType.MessageService,
            MessageType.Text,
            MessageSubType.Normal,
            null,
            new TMessageActionSetMessagesTTL
            {
                Period = evt.TtlPeriod ?? 0
            },
            MessageActionType.SetMessagesTtl,
            Post: true
        );
        
        var command = new StartSendMessageCommand(
            TempId.New,
            evt.RequestInfo,
            [new SendMessageItem(messageItem)]
        );
        
        Publish(command);
        await CompleteAsync(cancellationToken);
    }
}
