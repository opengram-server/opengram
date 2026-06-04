namespace MyTelegram.Domain.Aggregates.PushUpdates;

//public class UpdatesAggregate : AggregateRoot<UpdatesAggregate, UpdatesId>
//{
//    private readonly UpdatesState _state = new();
//    public UpdatesAggregate(UpdatesId id) : base(id)
//    {
//        Register(_state);
//    }

//    public void Create(long ownerPeerId, long? excludeAuthKeyId, long? excludeUserId, long? onlySendToThisAuthKeyId,
//        PtsType ptsType, int pts, int? messageId, int date, long seqNo,
//        byte[] updates,
//        List<long>? users, List<long>? chats)
//    {
//        Emit(new UpdatesCreatedEvent(ownerPeerId,
//            excludeAuthKeyId,
//            excludeUserId,
//            onlySendToThisAuthKeyId,
//            ptsType,
//            pts,
//            messageId,
//            date,
//            seqNo,
//            updates,
//            users,
//            chats
//            ));
//    }
//}

public class PushUpdatesAggregate : AggregateRoot<PushUpdatesAggregate, PushUpdatesId>
{
    private readonly PushUpdatesState _state = new();

    public PushUpdatesAggregate(PushUpdatesId id) : base(id)
    {
        Register(_state);
    }

    //public void Create(
    //    Peer toPeer,
    //    long excludeAuthKeyId,
    //    long excludeUid,
    //    long onlySendToThisAuthKeyId,
    //    byte[] data,
    //    int? messageId,
    //    int pts,
    //    PtsType ptsType,
    //    long seqNo,
    //    List<long> peerIdList)
    //{
    //    Emit(new PushUpdatesCreatedEvent(toPeer,
    //        excludeAuthKeyId,
    //        excludeUid,
    //        onlySendToThisAuthKeyId,
    //        data,
    //        messageId,
    //        pts,
    //        ptsType,
    //        seqNo,
    //        DateTime.UtcNow.ToTimestamp(),
    //        peerIdList
    //        ));
    //}

    public void CreateEncryptedPushUpdate(long inboxOwnerPeerId,
        byte[] data,
        int qts,
        long inboxOwnerPermAuthKeyId)
    {
        Emit(new EncryptedPushUpdatesCreatedEvent(inboxOwnerPeerId,
            data,
            qts,
            inboxOwnerPermAuthKeyId,
            DateTime.UtcNow.ToTimestamp()));
    }
}
