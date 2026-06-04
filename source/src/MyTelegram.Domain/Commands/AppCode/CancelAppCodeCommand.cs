namespace MyTelegram.Domain.Commands.AppCode;

public class CancelAppCodeCommand(
    AppCodeId aggregateId,
    RequestInfo requestInfo,
    string phoneNumber,
    string phoneCodeHash)
    : RequestCommand2<AppCodeAggregate, AppCodeId, IExecutionResult>(aggregateId, requestInfo)
{
    public string PhoneCodeHash { get; } = phoneCodeHash;
    public string PhoneNumber { get; } = phoneNumber;
}