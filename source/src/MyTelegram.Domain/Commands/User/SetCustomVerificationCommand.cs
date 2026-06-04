namespace MyTelegram.Domain.Commands.User;

public class SetCustomVerificationCommand : RequestCommand2<UserAggregate, UserId, IExecutionResult>
{
    public SetCustomVerificationCommand(
        UserId aggregateId,
        RequestInfo requestInfo,
        bool enabled,
        long? botUserId,
        long targetUserId,
        long iconEmojiId,
        string description,
        string? customDescription) : base(aggregateId, requestInfo)
    {
        Enabled = enabled;
        BotUserId = botUserId;
        TargetUserId = targetUserId;
        IconEmojiId = iconEmojiId;
        Description = description;
        CustomDescription = customDescription;
    }

    public bool Enabled { get; }
    public long? BotUserId { get; }
    public long TargetUserId { get; }
    public long IconEmojiId { get; }
    public string Description { get; }
    public string? CustomDescription { get; }
}
