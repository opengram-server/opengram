using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;

namespace MyTelegram.Domain.CommandHandlers.StarGift;

public class CompleteStarGiftCommandHandler : CommandHandler<StarGiftAggregate, StarGiftId, CompleteStarGiftCommand>
{
    public override Task ExecuteAsync(StarGiftAggregate aggregate, CompleteStarGiftCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[CompleteStarGiftCommandHandler] Completing gift. AggregateId: {aggregate.Id.Value}");
        aggregate.CompleteStarGift(command.RequestInfo);
        Console.WriteLine($"[CompleteStarGiftCommandHandler] Gift completed. AggregateId: {aggregate.Id.Value}");
        return Task.CompletedTask;
    }
}
