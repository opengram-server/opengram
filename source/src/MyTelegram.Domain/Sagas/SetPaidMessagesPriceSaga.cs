namespace MyTelegram.Domain.Sagas;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<SetPaidMessagesPriceSagaId>))]
public class SetPaidMessagesPriceSagaId(string value) : SingleValueObject<string>(value), ISagaId;

public class SetPaidMessagesPriceSagaLocator : DefaultSagaLocator<SetPaidMessagesPriceSaga, SetPaidMessagesPriceSagaId>
{
    protected override SetPaidMessagesPriceSagaId CreateSagaId(string requestId)
    {
        return new SetPaidMessagesPriceSagaId(requestId);
    }
}

/// <summary>
/// Saga to send service message when paid messages price is changed
/// </summary>
public class SetPaidMessagesPriceSaga(SetPaidMessagesPriceSagaId id, IEventStore eventStore, IIdGenerator idGenerator)
    : MyInMemoryAggregateSaga<SetPaidMessagesPriceSaga, SetPaidMessagesPriceSagaId, SetPaidMessagesPriceSagaLocator>(id, eventStore),
        ISagaIsStartedBy<ChannelAggregate, ChannelId, ChannelPaidMessagesPriceUpdatedEvent>
{
    public async Task HandleAsync(
        IDomainEvent<ChannelAggregate, ChannelId, ChannelPaidMessagesPriceUpdatedEvent> domainEvent,
        ISagaContext sagaContext,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        
        // Generate message ID for the service message
        var messageId = await idGenerator.NextIdAsync(IdType.MessageId, evt.ChannelId, cancellationToken: cancellationToken);
        
        var channelPeer = new Peer(PeerType.Channel, evt.ChannelId);
        var senderPeer = new Peer(PeerType.User, evt.RequestInfo.UserId);
        
        // Create service message with PaidMessagesPrice action
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
            new TMessageActionPaidMessagesPrice
            {
                Stars = evt.SendPaidMessagesStars ?? 0,
                BroadcastMessagesAllowed = evt.BroadcastMessagesAllowed
            },
            MessageActionType.PaidMessagesPrice,
            Post: true
        );
        
        // Use empty RequestInfo to avoid duplicate RPC response
        var command = new StartSendMessageCommand(
            TempId.New,
            RequestInfo.Empty,
            [new SendMessageItem(messageItem)]
        );
        
        Publish(command);
        await CompleteAsync(cancellationToken);
    }
}
