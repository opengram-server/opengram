using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;

namespace MyTelegram.Domain.CommandHandlers.StarGift;

public class ConvertStarGiftCommandHandler : CommandHandler<StarGiftAggregate, StarGiftId, ConvertStarGiftCommand>
{
    public override Task ExecuteAsync(StarGiftAggregate aggregate, ConvertStarGiftCommand command, CancellationToken cancellationToken)
    {
        aggregate.ConvertStarGift(command.RequestInfo, command.ConvertDate);
        return Task.CompletedTask;
    }
}
