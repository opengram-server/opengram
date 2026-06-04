using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Commands.StarGift;

public class PurchaseResaleStarGiftCommand(
    StarGiftId aggregateId,
    RequestInfo requestInfo,
    long buyerUserId,
    long recipientUserId,
    long priceStars,
    int date) : Command<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId)
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long BuyerUserId { get; } = buyerUserId;
    public long RecipientUserId { get; } = recipientUserId;
    public long PriceStars { get; } = priceStars;
    public int Date { get; } = date;
}
