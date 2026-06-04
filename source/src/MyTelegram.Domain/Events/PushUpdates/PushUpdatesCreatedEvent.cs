namespace MyTelegram.Domain.Events.PushUpdates;

public class PushUpdatesCreatedEvent(
    long ownerPeerId,
    long? excludeAuthKeyId,
    long? excludeUserId,
    long? onlySendToThisAuthKeyId,
    int pts,
    int? messageId,
    int date,
    long seqNo,
    byte[] updates,
    List<long>? users,
    List<long>? chats)
    : AggregateEvent<PushUpdatesAggregate, PushUpdatesId>
{
    /*PtsType ptsType,*/
    //PtsType = ptsType;

    public long OwnerPeerId { get; } = ownerPeerId;
    public long? ExcludeAuthKeyId { get; } = excludeAuthKeyId;
    public long? ExcludeUserId { get; } = excludeUserId;

    public long? OnlySendToThisAuthKeyId { get; } = onlySendToThisAuthKeyId;

    //public PtsType PtsType { get; }
    public int Pts { get; } = pts;
    public int? MessageId { get; } = messageId;
    public int Date { get; } = date;
    public long SeqNo { get; } = seqNo;
    public byte[] Updates { get; } = updates;
    public List<long>? Users { get; } = users;
    public List<long>? Chats { get; } = chats;
}

//public class UpdatesCreatedEvent : AggregateEvent<UpdatesAggregate, UpdatesId>
//{
//    public UpdatesCreatedEvent(long ownerPeerId, long? excludeAuthKeyId, long? excludeUserId, long? onlySendToThisAuthKeyId,
//        PtsType ptsType, int pts, int? messageId, int date, long seqNo,
//        byte[] updates,
//        List<long>? users, List<long>? chats)

//    {
//        OwnerPeerId = ownerPeerId;
//        ExcludeAuthKeyId = excludeAuthKeyId;
//        ExcludeUserId = excludeUserId;
//        OnlySendToThisAuthKeyId = onlySendToThisAuthKeyId;
//        PtsType = ptsType;
//        Pts = pts;
//        MessageId = messageId;
//        Date = date;
//        SeqNo = seqNo;
//        Updates = updates;
//        Users = users;
//        Chats = chats;
//    }

//    public long OwnerPeerId { get; }
//    public long? ExcludeAuthKeyId { get; }
//    public long? ExcludeUserId { get; }
//    public long? OnlySendToThisAuthKeyId { get; }
//    public PtsType PtsType { get; }
//    public int Pts { get; }
//    public int? MessageId { get; }
//    public int Date { get; }
//    public long SeqNo { get; }
//    public byte[] Updates { get; }
//    public List<long>? Users { get; }
//    public List<long>? Chats { get; }
//}