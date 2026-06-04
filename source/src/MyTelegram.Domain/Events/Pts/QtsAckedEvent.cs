namespace MyTelegram.Domain.Events.Pts;

public class QtsAckedEvent(
    long peerId,
    long permAuthKeyId,
    long msgId,
    int qts,
    long globalSeqNo,
    Peer toPeer,
    bool isFromGetDifference
    )
    : AggregateEvent<PtsAggregate, PtsId>
{
    public long GlobalSeqNo { get; } = globalSeqNo;
    public long MsgId { get; } = msgId;
    public long PeerId { get; } = peerId;
    public long PermAuthKeyId { get; } = permAuthKeyId;
    public int Qts { get; } = qts;
    public Peer ToPeer { get; } = toPeer;
    public bool IsFromGetDifference { get; } = isFromGetDifference;
}