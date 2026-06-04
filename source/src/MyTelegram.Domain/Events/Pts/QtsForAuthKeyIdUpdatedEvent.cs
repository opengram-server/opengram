namespace MyTelegram.Domain.Events.Pts;

public class QtsForAuthKeyIdUpdatedEvent(long peerId,
    long permAuthKeyId,
    int qts, long globalSeqNo) : AggregateEvent<PtsAggregate, PtsId>
{
    public long PeerId { get; } = peerId;
    public long PermAuthKeyId { get; } = permAuthKeyId;
    public int Qts { get; } = qts;
    public long GlobalSeqNo { get; } = globalSeqNo;
}