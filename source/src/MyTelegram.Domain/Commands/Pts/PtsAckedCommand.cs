namespace MyTelegram.Domain.Commands.Pts;

public class PtsAckedCommand(
    PtsId aggregateId,
    long peerId,
    long permAuthKeyId,
    long msgId,
    int pts,
    long globalSeqNo,
    Peer toPeer,
    bool isFromGetDifference = false
    )
    : Command<PtsAggregate, PtsId, IExecutionResult>(aggregateId)
{
    public long GlobalSeqNo { get; } = globalSeqNo;
    public long MsgId { get; } = msgId;
    public long PeerId { get; } = peerId;
    public long PermAuthKeyId { get; } = permAuthKeyId;
    public int Pts { get; } = pts;
    public Peer ToPeer { get; } = toPeer;
    public bool IsFromGetDifference { get; } = isFromGetDifference;
}
