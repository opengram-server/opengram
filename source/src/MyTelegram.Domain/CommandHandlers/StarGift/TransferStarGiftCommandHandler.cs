using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;

namespace MyTelegram.Domain.CommandHandlers.StarGift;

public class TransferStarGiftCommandHandler : CommandHandler<StarGiftAggregate, StarGiftId, TransferStarGiftCommand>
{
    public override Task ExecuteAsync(StarGiftAggregate aggregate, TransferStarGiftCommand command, CancellationToken cancellationToken)
    {
        aggregate.TransferStarGift(command.RequestInfo, command.NewOwnerId, command.TransferDate);
        return Task.CompletedTask;
    }
}
