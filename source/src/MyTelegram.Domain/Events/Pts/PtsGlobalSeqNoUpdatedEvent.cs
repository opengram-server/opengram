namespace MyTelegram.Domain.Events.Pts;

public class PtsGlobalSeqNoUpdatedEvent(
    long peerId,
    long permAuthKeyId,
    long globalSeqNo)
    : AggregateEvent<PtsAggregate, PtsId>
{
    public long GlobalSeqNo { get; } = globalSeqNo;
    public long PeerId { get; } = peerId;
    public long PermAuthKeyId { get; } = permAuthKeyId;
}
