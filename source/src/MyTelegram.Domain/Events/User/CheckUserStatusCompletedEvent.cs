namespace MyTelegram.Domain.Events.User;

public class CheckUserStatusCompletedEvent(
    RequestInfo requestInfo,
    long userId,
    long accessHash,
    string phoneNumber,
    string firstName,
    string? lastName,
    bool hasPassword,
    bool isUserLocked)
    : RequestAggregateEvent2<UserAggregate, UserId>(requestInfo)
{
    //string userName,


    public string FirstName { get; } = firstName;
    public bool HasPassword { get; } = hasPassword;
    public bool IsUserLocked { get; } = isUserLocked;
    public string? LastName { get; } = lastName;
    public string PhoneNumber { get; } = phoneNumber;

    public long UserId { get; } = userId;
    public long AccessHash { get; } = accessHash;
}
