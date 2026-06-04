namespace MyTelegram.Domain.Commands.User;

public class CreateBotVerifierCommand : Command<UserAggregate, UserId, IExecutionResult>
{
    public CreateBotVerifierCommand(
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
