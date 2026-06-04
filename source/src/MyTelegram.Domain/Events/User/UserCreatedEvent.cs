namespace MyTelegram.Domain.Events.User;

public class UserCreatedEvent(
    RequestInfo requestInfo,
    long userId,
    long accessHash,
    string phoneNumber,
    string firstName,
    string? lastName,
    string? userName,
    bool bot,
    int? botInfoVersion,
    int accountTtl,
    DateTime creationTime)
    : RequestAggregateEvent2<UserAggregate, UserId>(requestInfo)
{
    public long AccessHash { get; } = accessHash;
    public int AccountTtl { get; } = accountTtl;
    public bool Bot { get; } = bot;
    public int? BotInfoVersion { get; } = botInfoVersion;
    public DateTime CreationTime { get; } = creationTime;
    public string FirstName { get; } = firstName;
    public string? LastName { get; } = lastName;
    public string? UserName { get; } = userName;

    public string PhoneNumber { get; } = phoneNumber;
    public long UserId { get; } = userId;
}
