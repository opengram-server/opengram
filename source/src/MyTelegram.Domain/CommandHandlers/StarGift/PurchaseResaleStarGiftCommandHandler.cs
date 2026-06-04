using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;

namespace MyTelegram.Domain.CommandHandlers.StarGift;

public class PurchaseResaleStarGiftCommandHandler : CommandHandler<StarGiftAggregate, StarGiftId, IExecutionResult, PurchaseResaleStarGiftCommand>
{
    private readonly ILogger<PurchaseResaleStarGiftCommandHandler> _logger;

    public PurchaseResaleStarGiftCommandHandler(ILogger<PurchaseResaleStarGiftCommandHandler> logger)
    {
        _logger = logger;
    }

    public override async Task<IExecutionResult> ExecuteCommandAsync(
        StarGiftAggregate aggregate,
        PurchaseResaleStarGiftCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing resale purchase: AggregateId={AggregateId}, Buyer={Buyer}, Recipient={Recipient}, Price={Price}",
            aggregate.Id, command.BuyerUserId, command.RecipientUserId, command.PriceStars);

        // Выполняем покупку на агрегате
        aggregate.PurchaseResaleGift(
            command.RequestInfo,
            command.BuyerUserId,
            command.RecipientUserId,
            command.PriceStars,
            command.Date
        );

        _logger.LogInformation("Resale gift purchased: AggregateId={AggregateId}", aggregate.Id);

        return ExecutionResult.Success();
    }
}
