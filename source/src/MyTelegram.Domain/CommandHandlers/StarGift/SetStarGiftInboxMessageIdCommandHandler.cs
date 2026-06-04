using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;

namespace MyTelegram.Domain.CommandHandlers.StarGift;

public class SetStarGiftInboxMessageIdCommandHandler : CommandHandler<StarGiftAggregate, StarGiftId, SetStarGiftInboxMessageIdCommand>
{
    public override Task ExecuteAsync(StarGiftAggregate aggregate, SetStarGiftInboxMessageIdCommand command, CancellationToken cancellationToken)
    {
        aggregate.SetInboxMessageId(command.InboxMessageId);
        return Task.CompletedTask;
    }
}
