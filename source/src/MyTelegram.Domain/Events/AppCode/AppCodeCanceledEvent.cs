namespace MyTelegram.Domain.Events.AppCode;

public class AppCodeCanceledEvent(
    RequestInfo requestInfo,
    string phoneNumber,
    string phoneCodeHash)
    : RequestAggregateEvent2<AppCodeAggregate, AppCodeId>(requestInfo)
{
    public string PhoneCodeHash { get; } = phoneCodeHash;

    public string PhoneNumber { get; } = phoneNumber;
}