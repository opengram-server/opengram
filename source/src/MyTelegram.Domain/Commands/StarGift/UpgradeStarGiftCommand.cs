using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Commands.StarGift;

public class UpgradeStarGiftCommand(
    StarGiftId aggregateId,
    RequestInfo requestInfo,
    int upgradeMsgId,
    int upgradeDate,
    string uniqueId,
    byte[]? attributes)
    : RequestCommand2<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId, requestInfo)
{
    public int UpgradeMsgId { get; } = upgradeMsgId;
    public int UpgradeDate { get; } = upgradeDate;
    public string UniqueId { get; } = uniqueId;
    public byte[]? Attributes { get; } = attributes;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(RequestInfo.ReqMsgId);
    }
}
