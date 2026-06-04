using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Events.StarGift;

public class StarGiftConvertedEvent(
    RequestInfo requestInfo,
    long convertStars,
    int convertDate)
    : RequestAggregateEvent2<StarGiftAggregate, StarGiftId>(requestInfo)
{
    public long ConvertStars { get; } = convertStars;
    public int ConvertDate { get; } = convertDate;
}
