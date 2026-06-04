namespace MyTelegram.Domain.Commands.AppCode;

public class CheckSignInCodeCommand(
    AppCodeId aggregateId,
    RequestInfo requestInfo,
    string code,
    long userId)
    : RequestCommand2<AppCodeAggregate, AppCodeId, IExecutionResult>(aggregateId, requestInfo)
{
    //string phoneNumber,
    //string phoneCodeHash,
    //PhoneCodeHash = phoneCodeHash;

    public string Code { get; } = code;

    //public string PhoneCodeHash { get; }
    public long UserId { get; } = userId;
}
