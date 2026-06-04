using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Events.StarGift;

public class StarGiftPinnedToggledEvent(
    RequestInfo requestInfo,
    bool pinned)
    : RequestAggregateEvent2<StarGiftAggregate, StarGiftId>(requestInfo)
{
    public bool Pinned { get; } = pinned;
}
