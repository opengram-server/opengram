namespace MyTelegram.Domain.Commands.Pts;

public class UpdateQtsForAuthKeyIdCommand(
    PtsId aggregateId,
    long peerId,
    long permAuthKeyId,
    int newQts,
    long globalSeqNo)
    : Command<PtsAggregate, PtsId, IExecutionResult>(aggregateId)
{
    public long PeerId { get; } = peerId;
    public long PermAuthKeyId { get; } = permAuthKeyId;
    public int NewQts { get; } = newQts;
    public long GlobalSeqNo { get; } = globalSeqNo;
}