namespace MyTelegram.Domain.Commands.Pts;

public class UpdatePtsForAuthKeyIdCommand(
    PtsId aggregateId,
    long peerId,
    long permAuthKeyId,
    int newPts,
    int changedUnreadCount,
    long globalSeqNo)
    : Command<PtsAggregate, PtsId, IExecutionResult>(aggregateId)
{
    public long PeerId { get; } = peerId;
    public long PermAuthKeyId { get; } = permAuthKeyId;
    public int NewPts { get; } = newPts;
    public int ChangedUnreadCount { get; } = changedUnreadCount;
    public long GlobalSeqNo { get; } = globalSeqNo;
}