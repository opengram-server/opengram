using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Domain.Events.User;

public class BusinessIntroUpdatedEvent(long userId, BusinessIntro businessIntro) : AggregateEvent<UserAggregate, UserId>
{
    public long UserId { get; } = userId;
    public BusinessIntro BusinessIntro { get; } = businessIntro;
}
