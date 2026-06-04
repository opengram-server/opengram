namespace MyTelegram.Domain.Commands.Dialog;

public class SetDialogPinnedMsgIdCommand(
    DialogId aggregateId,
    long reqMsgId,
    int pinnedMsgId)
    : DistinctCommand<DialogAggregate, DialogId, IExecutionResult>(aggregateId)
{
    public int PinnedMsgId { get; } = pinnedMsgId;

    public long ReqMsgId { get; } = reqMsgId;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(ReqMsgId);
        yield return BitConverter.GetBytes(PinnedMsgId);
    }
}
