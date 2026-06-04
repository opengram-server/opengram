using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Events.StarGift;

public class StarGiftUpgradedEvent(
    RequestInfo requestInfo,
    int upgradeMsgId,
    int upgradeDate,
    string uniqueId,
    byte[]? attributes)
    : RequestAggregateEvent2<StarGiftAggregate, StarGiftId>(requestInfo)
{
    public int UpgradeMsgId { get; } = upgradeMsgId;
    public int UpgradeDate { get; } = upgradeDate;
    public string UniqueId { get; } = uniqueId;
    public byte[]? Attributes { get; } = attributes;
}
