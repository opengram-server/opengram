namespace MyTelegram.Domain.Sagas;

public class UpdateContactProfilePhotoSaga(
    UpdateContactProfilePhotoSagaId id,
    IEventStore eventStore,
    IIdGenerator idGenerator)
    : MyInMemoryAggregateSaga<UpdateContactProfilePhotoSaga,
            UpdateContactProfilePhotoSagaId, UpdateContactProfilePhotoSagaLocator>(id, eventStore),
        ISagaIsStartedBy<ContactAggregate, ContactId, ContactProfilePhotoChangedEvent>
{
    public async Task HandleAsync(IDomainEvent<ContactAggregate, ContactId, ContactProfilePhotoChangedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.Suggest)
        {
            var ownerPeerId = domainEvent.AggregateEvent.SelfUserId;

            var outMessageId = await idGenerator.NextIdAsync(IdType.MessageId, ownerPeerId, cancellationToken: cancellationToken);
            var randomId = Random.Shared.NextInt64();
            var ownerPeer = new Peer(PeerType.User, ownerPeerId);
            var senderPeer = new Peer(PeerType.User, ownerPeerId);
            var toPeer = new Peer(PeerType.User, domainEvent.AggregateEvent.TargetUserId);
            var messageItem = new MessageItem(ownerPeer, toPeer, senderPeer,
                senderPeer.PeerId,
                outMessageId,
                string.Empty,
                DateTime.UtcNow.ToTimestamp(),
                randomId,
                true,
                SendMessageType.MessageService,
                MessageType.Text,
                MessageSubType.None,
                MessageAction: domainEvent.AggregateEvent.SuggestPhoto != null ? new TMessageActionSuggestProfilePhoto
                {
                    Photo = domainEvent.AggregateEvent.SuggestPhoto
                } : null
            );
            var command = new StartSendMessageCommand(TempId.New,
                domainEvent.AggregateEvent.RequestInfo with { RequestId = Guid.NewGuid() },
                [new SendMessageItem(messageItem)]);

            Publish(command);
        }

        await CompleteAsync(cancellationToken);
    }
}