using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;

namespace MyTelegram.Domain.CommandHandlers.StarGift;

public class UpgradeStarGiftCommandHandler : CommandHandler<StarGiftAggregate, StarGiftId, UpgradeStarGiftCommand>
{
    public override Task ExecuteAsync(StarGiftAggregate aggregate, UpgradeStarGiftCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpgradeStarGift(command.RequestInfo, command.UpgradeMsgId, command.UpgradeDate, command.UniqueId, command.Attributes);
        return Task.CompletedTask;
    }
}
