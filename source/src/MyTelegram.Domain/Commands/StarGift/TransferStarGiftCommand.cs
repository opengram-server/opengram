using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Commands.StarGift;

public class TransferStarGiftCommand(
    StarGiftId aggregateId,
    RequestInfo requestInfo,
    long newOwnerId,
    int transferDate)
    : RequestCommand2<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId, requestInfo)
{
    public long NewOwnerId { get; } = newOwnerId;
    public int TransferDate { get; } = transferDate;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(RequestInfo.ReqMsgId);
        yield return BitConverter.GetBytes(NewOwnerId);
    }
}
