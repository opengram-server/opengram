namespace MyTelegram.Domain.Events.User;

public class BotVerifierCreatedEvent : AggregateEvent<UserAggregate, UserId>
{
    public BotVerifierCreatedEvent(
        long botUserId,
        long iconEmojiId,
        string companyName,
        bool canModifyCustomDescription)
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
