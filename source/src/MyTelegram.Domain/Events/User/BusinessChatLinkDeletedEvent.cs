namespace MyTelegram.Domain.Events.User;

public class BusinessChatLinkDeletedEvent(long userId, string linkId) : AggregateEvent<UserAggregate, UserId>
{
    public long UserId { get; } = userId;
    public string LinkId { get; } = linkId;
}
