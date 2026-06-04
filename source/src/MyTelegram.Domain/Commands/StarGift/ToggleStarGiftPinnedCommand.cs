using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Commands.StarGift;

public class ToggleStarGiftPinnedCommand(
    StarGiftId aggregateId,
    RequestInfo requestInfo,
    bool pinned)
    : RequestCommand2<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool Pinned { get; } = pinned;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(RequestInfo.ReqMsgId);
    }
}
