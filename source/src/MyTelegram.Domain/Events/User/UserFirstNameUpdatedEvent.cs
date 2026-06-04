namespace MyTelegram.Domain.Events.User;

public class UserFirstNameUpdatedEvent(long userId, string firstName) : AggregateEvent<UserAggregate, UserId>
{
    public long UserId { get; } = userId;
    public string FirstName { get; } = firstName;
}