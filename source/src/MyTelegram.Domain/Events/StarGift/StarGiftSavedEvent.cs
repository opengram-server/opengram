using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Events.StarGift;

public class StarGiftSavedEvent(
    RequestInfo requestInfo,
    bool saved,
    long? savedId)
    : RequestAggregateEvent2<StarGiftAggregate, StarGiftId>(requestInfo)
{
    public bool Saved { get; } = saved;
    public long? SavedId { get; } = savedId;
}
