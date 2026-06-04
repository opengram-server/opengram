namespace MyTelegram.Domain.Events.AppCode;

public class AppCodeCreatedEvent(
    RequestInfo requestInfo,
    long userId,
    string phoneNumber,
    string code,
    int expire,
    string phoneCodeHash,
    long creationTime)
    : RequestAggregateEvent2<AppCodeAggregate, AppCodeId>(requestInfo)
{
    public string Code { get; } = code;
    public long CreationTime { get; } = creationTime;
    public int Expire { get; } = expire;
    public string PhoneCodeHash { get; } = phoneCodeHash;
    public string PhoneNumber { get; } = phoneNumber;
    public long UserId { get; } = userId;
}