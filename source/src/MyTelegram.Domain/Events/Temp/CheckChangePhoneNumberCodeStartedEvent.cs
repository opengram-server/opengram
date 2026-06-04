namespace MyTelegram.Domain.Events.Temp;

public class CheckChangePhoneNumberCodeStartedEvent(RequestInfo requestInfo, string phoneNumber, string phoneCodeHash, string code) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public string PhoneNumber { get; } = phoneNumber;
    public string PhoneCodeHash { get; } = phoneCodeHash;
    public string Code { get; } = code;
}