using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Commands.StarGift;

public class SendStarGiftCommand(
    StarGiftId aggregateId,
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
    int date)
    : RequestCommand2<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId, requestInfo)
{
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

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(RequestInfo.ReqMsgId);
        yield return BitConverter.GetBytes(GiftId);
        yield return BitConverter.GetBytes(FromUserId);
        yield return BitConverter.GetBytes(ToUserId);
    }
}
