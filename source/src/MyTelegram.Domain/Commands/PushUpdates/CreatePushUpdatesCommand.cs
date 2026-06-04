namespace MyTelegram.Domain.Commands.PushUpdates;

public class CreatePushUpdatesCommand(
    PushUpdatesId aggregateId,
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
    : Command<PushUpdatesAggregate, PushUpdatesId, IExecutionResult>(aggregateId)
{
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

    /*PtsType ptsType,*/
    //PtsType = ptsType;
}
