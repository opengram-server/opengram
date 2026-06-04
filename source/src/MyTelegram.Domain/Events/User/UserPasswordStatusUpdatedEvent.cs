namespace MyTelegram.Domain.Events.User;

public class UserPasswordStatusUpdatedEvent(
    long userId,
    bool hasPassword) : AggregateEvent<UserAggregate, UserId>
{
    public long UserId { get; } = userId;
    public bool HasPassword { get; } = hasPassword;
}
