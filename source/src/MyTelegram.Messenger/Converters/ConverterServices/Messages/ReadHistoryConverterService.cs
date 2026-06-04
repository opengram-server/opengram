namespace MyTelegram.Messenger.Converters.ConverterServices.Messages;

internal sealed class ReadHistoryConverterService : IReadHistoryConverterService, ITransientDependency
{
    public IUpdates ToReadHistoryUpdates(ReadHistoryCompletedSagaEvent aggregateEvent)
    {
        var peer = aggregateEvent.ReaderToPeer.PeerType == PeerType.User
            ? new TPeerUser { UserId = aggregateEvent.ReaderUserId }
            : aggregateEvent.ReaderToPeer.ToPeer();
        var updateReadHistoryOutbox = new TUpdateReadHistoryOutbox
        {
            Pts = aggregateEvent.SenderPts,
            MaxId = aggregateEvent.SenderMessageId,
            PtsCount = 1,
            Peer = peer
        };

        var updates = new TUpdates
        {
            Chats = [],
            Date = DateTime.UtcNow.ToTimestamp(),
            Updates = new TVector<IUpdate>(updateReadHistoryOutbox),
            Users = [],
            Seq = 0
        };

        return updates;
    }

    public IUpdates ToReadHistoryUpdates(UpdateOutboxMaxIdCompletedSagaEvent eventData)
    {
        var updateReadHistoryOutbox = new TUpdateReadHistoryOutbox
        {
            Pts = eventData.Pts,
            MaxId = eventData.MaxId,
            PtsCount = 1,
            Peer = eventData.ToPeerId.ToUserPeer().ToPeer()
        };

        var updates = new TUpdates
        {
            Chats = [],
            Date = DateTime.UtcNow.ToTimestamp(),
            Updates = new TVector<IUpdate>(updateReadHistoryOutbox),
            Users = [],
            Seq = 0
        };

        return updates;
    }
}
