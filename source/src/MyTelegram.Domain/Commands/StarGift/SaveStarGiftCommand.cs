using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Commands.StarGift;

public class SaveStarGiftCommand(
    StarGiftId aggregateId,
    RequestInfo requestInfo,
    bool save,
    long? savedId = null)
    : RequestCommand2<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool Save { get; } = save;
    public long? SavedId { get; } = savedId;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(RequestInfo.ReqMsgId);
    }
}
