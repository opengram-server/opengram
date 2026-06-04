namespace MyTelegram.Domain.Commands.Pts;

public class UpdateQtsCommand(
    PtsId aggregateId,
    long peerId,
    long permAuthKeyId,
    int newQts, long globalSeqNo) : Command<PtsAggregate, PtsId, IExecutionResult>(aggregateId)
{
    public int NewQts { get; } = newQts;
    public long GlobalSeqNo { get; } = globalSeqNo;

    public long PeerId { get; } = peerId;
    public long PermAuthKeyId { get; } = permAuthKeyId;
}
