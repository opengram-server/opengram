namespace MyTelegram.Domain.Events.Pts;

public class PtsForAuthKeyIdUpdatedEvent(
    long peerId,
    long permAuthKeyId,
    int pts,
    int changedUnreadCount,
    long globalSeqNo)
    : AggregateEvent<PtsAggregate, PtsId>
{
    public long PeerId { get; } = peerId;
    public long PermAuthKeyId { get; } = permAuthKeyId;
    public int Pts { get; } = pts;
    public int ChangedUnreadCount { get; } = changedUnreadCount;
    public long GlobalSeqNo { get; } = globalSeqNo;
}