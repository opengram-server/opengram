using MyTelegram.Domain.Aggregates.StarGift;

namespace MyTelegram.Domain.Commands.StarGift;

public class SetStarGiftOutboxMessageIdCommand(
    StarGiftId aggregateId,
    int outboxMessageId)
    : Command<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId)
{
    public int OutboxMessageId { get; } = outboxMessageId;
}

public class SetStarGiftInboxMessageIdCommand(
    StarGiftId aggregateId,
    int inboxMessageId)
    : Command<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId)
{
    public int InboxMessageId { get; } = inboxMessageId;
}

public class InitiateStarGiftCommand(
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
    : Command<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId, new CommandId($"command-{requestInfo.RequestId.ToString().ToLower()}"))
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
}

public class CompleteStarGiftCommand(StarGiftId aggregateId, RequestInfo requestInfo)
    : Command<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId, new CommandId($"command-{requestInfo.RequestId.ToString().ToLower()}"))
{
    public RequestInfo RequestInfo { get; } = requestInfo;
}

public class ListStarGiftForResaleCommand(
    StarGiftId aggregateId,
    RequestInfo requestInfo,
    long resaleStars)
    : Command<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId, new CommandId($"command-{requestInfo.RequestId.ToString().ToLower()}"))
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long ResaleStars { get; } = resaleStars;
}

public class RemoveStarGiftFromResaleCommand(
    StarGiftId aggregateId,
    RequestInfo requestInfo)
    : Command<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId, new CommandId($"command-{requestInfo.RequestId.ToString().ToLower()}"))
{
    public RequestInfo RequestInfo { get; } = requestInfo;
}

public class BuyStarGiftFromResaleCommand(
    StarGiftId aggregateId,
    RequestInfo requestInfo,
    long buyerUserId,
    long recipientUserId,
    long resaleStars)
    : Command<StarGiftAggregate, StarGiftId, IExecutionResult>(aggregateId, new CommandId($"command-{requestInfo.RequestId.ToString().ToLower()}"))
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long BuyerUserId { get; } = buyerUserId;
    public long RecipientUserId { get; } = recipientUserId;
    public long ResaleStars { get; } = resaleStars;
}
