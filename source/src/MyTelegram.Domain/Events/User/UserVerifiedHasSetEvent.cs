namespace MyTelegram.Domain.Events.User;

public class UserVerifiedHasSetEvent(bool verified) : AggregateEvent<UserAggregate, UserId>
{
    public bool Verified { get; } = verified;
}
