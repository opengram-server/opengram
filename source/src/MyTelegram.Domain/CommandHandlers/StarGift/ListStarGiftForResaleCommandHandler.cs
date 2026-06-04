using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;

namespace MyTelegram.Domain.CommandHandlers.StarGift;

public class ListStarGiftForResaleCommandHandler : ICommandHandler<StarGiftAggregate, StarGiftId, IExecutionResult, ListStarGiftForResaleCommand>
{
    public Task<IExecutionResult> ExecuteCommandAsync(
        StarGiftAggregate aggregate,
        ListStarGiftForResaleCommand command,
        CancellationToken cancellationToken)
    {
        var currentDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        aggregate.ListForResale(command.RequestInfo, command.ResaleStars, (int)currentDate);
        return Task.FromResult(ExecutionResult.Success());
    }
}
