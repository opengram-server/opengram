using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Domain.Events.User;

public class BusinessWorkHoursUpdatedEvent(long userId, BusinessWorkHours workHours) : AggregateEvent<UserAggregate, UserId>
{
    public long UserId { get; } = userId;
    public BusinessWorkHours WorkHours { get; } = workHours;
}
