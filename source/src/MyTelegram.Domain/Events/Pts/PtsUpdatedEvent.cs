namespace MyTelegram.Domain.Events.Pts;

public class PtsUpdatedEvent(
    long peerId,
    long permAuthKeyId,
    int newPts,
    int date,
    long globalSeqNo,
    int changedUnreadCount,
    int? messageId
    )
    : AggregateEvent<PtsAggregate, PtsId>
{
    public int NewPts { get; } = newPts;
    public int Date { get; } = date;
    public long GlobalSeqNo { get; } = globalSeqNo;
    public int ChangedUnreadCount { get; } = changedUnreadCount;
    public int? MessageId { get; } = messageId;
    public long PeerId { get; } = peerId;
    public long PermAuthKeyId { get; } = permAuthKeyId;
}
