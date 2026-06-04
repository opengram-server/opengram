namespace MyTelegram.Domain.Events.Pts;

public class QtsUpdatedEvent(
    long peerId,
    long permAuthKeyId,
    int newQts, int date, long globalSeqNo) : AggregateEvent<PtsAggregate, PtsId>
{
    public int NewQts { get; } = newQts;
    public long GlobalSeqNo { get; } = globalSeqNo;
    public int Date { get; } = date;

    public long PeerId { get; } = peerId;
    public long PermAuthKeyId { get; } = permAuthKeyId;
}
