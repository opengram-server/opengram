using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Domain.Events.User;

public class BusinessChatLinkCreatedEvent(long userId, BusinessChatLink chatLink) : AggregateEvent<UserAggregate, UserId>
{
    public long UserId { get; } = userId;
    public BusinessChatLink ChatLink { get; } = chatLink;
}
