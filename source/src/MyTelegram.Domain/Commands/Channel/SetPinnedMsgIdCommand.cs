namespace MyTelegram.Domain.Commands.Channel;

public class SetPinnedMsgIdCommand(
    ChannelId aggregateId,
    long reqMsgId,
    int pinnedMsgId,
    bool pinned)
    : DistinctCommand<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId)
{
    public bool Pinned { get; } = pinned;
    public int PinnedMsgId { get; } = pinnedMsgId;
    public long ReqMsgId { get; } = reqMsgId;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(ReqMsgId);
        yield return BitConverter.GetBytes(PinnedMsgId);
    }
}
