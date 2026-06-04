using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;

namespace MyTelegram.Domain.CommandHandlers.StarGift;

public class SaveStarGiftCommandHandler : CommandHandler<StarGiftAggregate, StarGiftId, SaveStarGiftCommand>
{
    public override Task ExecuteAsync(StarGiftAggregate aggregate, SaveStarGiftCommand command, CancellationToken cancellationToken)
    {
        aggregate.SaveStarGift(command.RequestInfo, command.Save, command.SavedId);
        return Task.CompletedTask;
    }
}
