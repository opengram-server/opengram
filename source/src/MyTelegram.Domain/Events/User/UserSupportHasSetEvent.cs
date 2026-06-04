namespace MyTelegram.Domain.Events.User;

public class UserSupportHasSetEvent(bool support) : AggregateEvent<UserAggregate, UserId>
{
    public bool Support { get; } = support;
}
