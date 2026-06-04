using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Events.StarGift;

public class StarGiftTransferredEvent(
    RequestInfo requestInfo,
    long newOwnerId,
    int transferDate)
    : RequestAggregateEvent2<StarGiftAggregate, StarGiftId>(requestInfo)
{
    public long NewOwnerId { get; } = newOwnerId;
    public int TransferDate { get; } = transferDate;
}
