namespace MyTelegram.Domain.Commands.Pts;

public class UpdatePtsCommand(
    PtsId aggregateId,
    long peerId,
    long permAuthKeyId,
    int newPts,
    long globalSeqNo,
    int changedUnreadCount,
    int? messageId
    )
    : Command<PtsAggregate, PtsId, IExecutionResult>(aggregateId)
{
    public int NewPts { get; } = newPts;
    public long GlobalSeqNo { get; } = globalSeqNo;
    public int ChangedUnreadCount { get; } = changedUnreadCount;
    public int? MessageId { get; } = messageId;
    public long PeerId { get; } = peerId;
    public long PermAuthKeyId { get; } = permAuthKeyId;
}