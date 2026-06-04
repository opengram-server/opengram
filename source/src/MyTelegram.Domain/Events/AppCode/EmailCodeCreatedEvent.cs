namespace MyTelegram.Domain.Events.AppCode;

public class EmailCodeCreatedEvent(RequestInfo requestInfo, long userId, string email, string code, int expire, AppCodeType appCodeType)
    : RequestAggregateEvent2<AppCodeAggregate, AppCodeId>(requestInfo)
{
    public long UserId { get; } = userId;
    public string Email { get; } = email;
    public string Code { get; } = code;
    public int Expire { get; } = expire;
    public AppCodeType AppCodeType { get; } = appCodeType;
}