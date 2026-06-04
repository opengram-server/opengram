using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Domain.Events.User;

public class BusinessLocationUpdatedEvent(long userId, BusinessLocation location) : AggregateEvent<UserAggregate, UserId>
{
    public long UserId { get; } = userId;
    public BusinessLocation Location { get; } = location;
}
