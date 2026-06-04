using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;

namespace MyTelegram.Domain.CommandHandlers.StarGift;

public class RemoveStarGiftFromResaleCommandHandler : ICommandHandler<StarGiftAggregate, StarGiftId, IExecutionResult, RemoveStarGiftFromResaleCommand>
{
    public Task<IExecutionResult> ExecuteCommandAsync(
        StarGiftAggregate aggregate,
        RemoveStarGiftFromResaleCommand command,
        CancellationToken cancellationToken)
    {
        var currentDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        aggregate.RemoveFromResale(command.RequestInfo, (int)currentDate);
        return Task.FromResult(ExecutionResult.Success());
    }
}
