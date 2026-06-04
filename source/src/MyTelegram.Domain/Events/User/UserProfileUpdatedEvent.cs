namespace MyTelegram.Domain.Events.User;

public class UserProfileUpdatedEvent(
    RequestInfo requestInfo,
    long userId,
    string firstName,
    string? lastName,
    string? about)
    : RequestAggregateEvent2<UserAggregate, UserId>(requestInfo)
{
    public string? About { get; } = about;
    public string FirstName { get; } = firstName;
    public string? LastName { get; } = lastName;

    public long UserId { get; } = userId;
}
