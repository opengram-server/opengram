using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;

namespace MyTelegram.Domain.CommandHandlers.StarGift;

public class SetStarGiftOutboxMessageIdCommandHandler : CommandHandler<StarGiftAggregate, StarGiftId, SetStarGiftOutboxMessageIdCommand>
{
    public override Task ExecuteAsync(StarGiftAggregate aggregate, SetStarGiftOutboxMessageIdCommand command, CancellationToken cancellationToken)
    {
        aggregate.SetOutboxMessageId(command.OutboxMessageId);
        return Task.CompletedTask;
    }
}
