using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Events.StarGift;

public class StarGiftOutboxMessageCreatedEvent(
    int outboxMessageId)
    : AggregateEvent<StarGiftAggregate, StarGiftId>
{
    public int OutboxMessageId { get; } = outboxMessageId;
}

public class StarGiftInboxMessageCreatedEvent(
    int inboxMessageId)
    : AggregateEvent<StarGiftAggregate, StarGiftId>
{
    public int InboxMessageId { get; } = inboxMessageId;
}

public class StarGiftInitiatedEvent(
    RequestInfo requestInfo,
    long giftId,
    long fromUserId,
    long toUserId,
    long? toPeerId,
    int messageId,
    long stars,
    long convertStars,
    string? message,
    bool nameHidden,
    bool canUpgrade,
    long? upgradeStars,
    byte[]? giftSticker,
    int date,
    string aggregateIdValue)
    : AggregateEvent<StarGiftAggregate, StarGiftId>
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long GiftId { get; } = giftId;
    public long FromUserId { get; } = fromUserId;
    public long ToUserId { get; } = toUserId;
    public long? ToPeerId { get; } = toPeerId;
    public int MessageId { get; } = messageId;
    public long Stars { get; } = stars;
    public long ConvertStars { get; } = convertStars;
    public string? Message { get; } = message;
    public bool NameHidden { get; } = nameHidden;
    public bool CanUpgrade { get; } = canUpgrade;
    public long? UpgradeStars { get; } = upgradeStars;
    public byte[]? GiftSticker { get; } = giftSticker;
    public int Date { get; } = date;
    public string AggregateIdValue { get; } = aggregateIdValue;
}

public class StarGiftListedForResaleEvent(
    RequestInfo requestInfo,
    long resaleStars,
    int resaleDate)
    : AggregateEvent<StarGiftAggregate, StarGiftId>
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long ResaleStars { get; } = resaleStars;
    public int ResaleDate { get; } = resaleDate;
}

public class StarGiftRemovedFromResaleEvent(
    RequestInfo requestInfo,
    int removeDate)
    : AggregateEvent<StarGiftAggregate, StarGiftId>
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public int RemoveDate { get; } = removeDate;
}

public class StarGiftSoldViaResaleEvent(
    RequestInfo requestInfo,
    long boughtByUserId,
    long resaleStars,
    int soldDate)
    : AggregateEvent<StarGiftAggregate, StarGiftId>
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long BoughtByUserId { get; } = boughtByUserId;
    public long ResaleStars { get; } = resaleStars;
    public int SoldDate { get; } = soldDate;
}
