namespace MyTelegram.Domain.Events.User;

public class UserAboutUpdatedEvent(long userId, string? about) : AggregateEvent<UserAggregate, UserId>
{
    public long UserId { get; } = userId;
    public string? About { get; } = about;
}