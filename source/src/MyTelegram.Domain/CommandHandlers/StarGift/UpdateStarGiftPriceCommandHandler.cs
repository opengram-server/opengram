using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;

namespace MyTelegram.Domain.CommandHandlers.StarGift;

public class UpdateStarGiftPriceCommandHandler : ICommandHandler<StarGiftAggregate, StarGiftId, IExecutionResult, UpdateStarGiftPriceCommand>
{
    public Task<IExecutionResult> ExecuteCommandAsync(
        StarGiftAggregate aggregate,
        UpdateStarGiftPriceCommand command,
        CancellationToken cancellationToken)
    {
        var currentDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        // If price > 0, list for resale, otherwise remove from resale
        if (command.Price > 0)
        {
            aggregate.ListForResale(command.RequestInfo, command.Price, (int)currentDate);
        }
        else
        {
            aggregate.RemoveFromResale(command.RequestInfo, (int)currentDate);
        }
        
        return Task.FromResult(ExecutionResult.Success());
    }
}
