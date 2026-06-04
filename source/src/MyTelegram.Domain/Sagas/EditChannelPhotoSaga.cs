namespace MyTelegram.Domain.Sagas;

public class
    EditChannelPhotoSaga(EditChannelPhotoSagaId id, IEventStore eventStore, IIdGenerator idGenerator)
    : MyInMemoryAggregateSaga<EditChannelPhotoSaga, EditChannelPhotoSagaId, EditChannelPhotoSagaLocator>(id,
            eventStore),
        ISagaIsStartedBy<ChannelAggregate, ChannelId, ChannelPhotoEditedEvent>
{
    public async Task HandleAsync(IDomainEvent<ChannelAggregate, ChannelId, ChannelPhotoEditedEvent> domainEvent,
        ISagaContext sagaContext,
        CancellationToken cancellationToken)
    {
        var outMessageId = await idGenerator.NextIdAsync(IdType.MessageId, domainEvent.AggregateEvent.ChannelId, cancellationToken: cancellationToken);
        //var aggregateId = MessageId.Create(domainEvent.AggregateEvent.ChannelId, outMessageId);
        var ownerPeer = new Peer(PeerType.Channel, domainEvent.AggregateEvent.ChannelId);
        var senderPeer = new Peer(PeerType.User, domainEvent.AggregateEvent.RequestInfo.UserId);
        Peer? sendAs = null;
        if (!domainEvent.AggregateEvent.Broadcast && domainEvent.AggregateEvent.LinkedChannelId != null)
        {
            sendAs = domainEvent.AggregateEvent.ChannelId.ToChannelPeer();
        }
        var messageItem = new MessageItem(
            ownerPeer,
            ownerPeer,
            senderPeer,
            senderPeer.PeerId,
            outMessageId,
            string.Empty,
            DateTime.UtcNow.ToTimestamp(),
            domainEvent.AggregateEvent.RandomId,
            true,
            SendMessageType.MessageService,
            MessageType.Text,
            MessageSubType.EditChannelPhoto,
            null,
            domainEvent.AggregateEvent.MessageAction,
            MessageActionType.ChatEditPhoto,
            Post: domainEvent.AggregateEvent.Broadcast,
            SendAs: sendAs
        );
        //var command = new CreateOutboxMessageCommand(aggregateId,
        //    domainEvent.AggregateEvent.RequestInfo,
        //    messageItem);
        var command = new StartSendMessageCommand(TempId.New,
            domainEvent.AggregateEvent.RequestInfo,
            [new SendMessageItem(messageItem)]);

        Publish(command);
        await CompleteAsync(cancellationToken);
    }
}
