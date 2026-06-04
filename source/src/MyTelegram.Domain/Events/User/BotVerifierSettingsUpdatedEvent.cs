namespace MyTelegram.Domain.Events.User;

public class BotVerifierSettingsUpdatedEvent : AggregateEvent<UserAggregate, UserId>
{
    public BotVerifierSettingsUpdatedEvent(
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
