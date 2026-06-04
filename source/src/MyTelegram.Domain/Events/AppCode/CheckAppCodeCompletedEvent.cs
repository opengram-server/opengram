namespace MyTelegram.Domain.Events.AppCode;

public class CheckAppCodeCompletedEvent(RequestInfo requestInfo, string? email, bool isValidCode)
    : RequestAggregateEvent2<AppCodeAggregate, AppCodeId>(requestInfo)
{
    public string? Email { get; } = email;
    public bool IsValidCode { get; } = isValidCode;
}