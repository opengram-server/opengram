using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Commands.StarGift;

public class UpdateStarGiftPriceCommand(
    StarGiftId aggregateId,
    RequestInfo requestInfo,
    long price)
    : Command<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId, new CommandId($"command-{requestInfo.RequestId.ToString().ToLower()}"))
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long Price { get; } = price;
}
