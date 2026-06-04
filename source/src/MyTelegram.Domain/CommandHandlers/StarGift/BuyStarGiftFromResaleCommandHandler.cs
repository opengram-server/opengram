using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;

namespace MyTelegram.Domain.CommandHandlers.StarGift;

public class BuyStarGiftFromResaleCommandHandler : ICommandHandler<StarGiftAggregate, StarGiftId, IExecutionResult, BuyStarGiftFromResaleCommand>
{
    public Task<IExecutionResult> ExecuteCommandAsync(
        StarGiftAggregate aggregate,
        BuyStarGiftFromResaleCommand command,
        CancellationToken cancellationToken)
    {
        var currentDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        aggregate.PurchaseResaleGift(command.RequestInfo, command.BuyerUserId, command.RecipientUserId, command.ResaleStars, (int)currentDate);
        return Task.FromResult(ExecutionResult.Success());
    }
}
