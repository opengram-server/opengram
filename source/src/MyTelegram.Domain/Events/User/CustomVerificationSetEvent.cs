namespace MyTelegram.Domain.Events.User;

public class CustomVerificationSetEvent : RequestAggregateEvent2<UserAggregate, UserId>
{
    public CustomVerificationSetEvent(
        RequestInfo requestInfo,
        long targetUserId,
        long botVerifierId,
        long iconEmojiId,
        string description,
        string? customDescription) : base(requestInfo)
    {
        TargetUserId = targetUserId;
        BotVerifierId = botVerifierId;
        IconEmojiId = iconEmojiId;
        Description = description;
        CustomDescription = customDescription;
    }

    public long TargetUserId { get; }
    public long BotVerifierId { get; }
    public long IconEmojiId { get; }
    public string Description { get; }
    public string? CustomDescription { get; }
}
