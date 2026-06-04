using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;

namespace MyTelegram.Domain.CommandHandlers.StarGift;

public class InitiateStarGiftCommandHandler : CommandHandler<StarGiftAggregate, StarGiftId, InitiateStarGiftCommand>
{
    public override Task ExecuteAsync(StarGiftAggregate aggregate, InitiateStarGiftCommand command, CancellationToken cancellationToken)
    {
        aggregate.InitiateStarGift(
            command.RequestInfo,
            command.GiftId,
            command.FromUserId,
            command.ToUserId,
            command.ToPeerId,
            command.MessageId,
            command.Stars,
            command.ConvertStars,
            command.Message,
            command.NameHidden,
            command.CanUpgrade,
            command.UpgradeStars,
            command.GiftSticker,
            command.Date
        );
        return Task.CompletedTask;
    }
}
