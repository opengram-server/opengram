using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Domain.Events.User;

public class BusinessAwayMessageUpdatedEvent(long userId, BusinessAwayMessage? awayMessage) 
    : AggregateEvent<UserAggregate, UserId>
{
    public long UserId { get; } = userId;
    public BusinessAwayMessage? AwayMessage { get; } = awayMessage;
}
