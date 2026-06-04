using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Domain.Events.User;

public class BusinessGreetingMessageUpdatedEvent(long userId, BusinessGreetingMessage greetingMessage) : AggregateEvent<UserAggregate, UserId>
{
    public long UserId { get; } = userId;
    public BusinessGreetingMessage GreetingMessage { get; } = greetingMessage;
}
