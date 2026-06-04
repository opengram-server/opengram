namespace MyTelegram.Domain.Commands.User;

public class UpdateBotVerifierSettingsCommand : Command<UserAggregate, UserId, IExecutionResult>
{
    public UpdateBotVerifierSettingsCommand(
        UserId aggregateId,
        long botUserId,
        long iconEmojiId,
        string companyName,
        bool canModifyCustomDescription) : base(aggregateId)
    {
        BotUserId = botUserId;
        IconEmojiId = iconEmojiId;
        CompanyName = companyName;
        CanModifyCustomDescription = canModifyCustomDescription;
    }

    public long BotUserId { get; }
    public long IconEmojiId { get; }
    public string CompanyName { get; }
    public bool CanModifyCustomDescription { get; }
}
