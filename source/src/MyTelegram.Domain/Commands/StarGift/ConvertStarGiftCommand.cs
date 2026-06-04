using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Commands.StarGift;

public class ConvertStarGiftCommand(
    StarGiftId aggregateId,
    RequestInfo requestInfo,
    int convertDate)
    : RequestCommand2<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId, requestInfo)
{
    public int ConvertDate { get; } = convertDate;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(RequestInfo.ReqMsgId);
    }
}
