namespace MyTelegram.Domain.Commands.AppCode;

public class CheckSignUpCodeCommand(
    AppCodeId aggregateId,
    RequestInfo requestInfo,
    string phoneCodeHash,
    long userId,
    long accessHash,
    string phoneNumber,
    string firstName,
    string? lastName
    )
    : RequestCommand2<AppCodeAggregate, AppCodeId, IExecutionResult>(aggregateId, requestInfo)
{
    //string phoneNumber,
    //string code, 
    //PhoneNumber = phoneNumber;
    //Code = code; 

    public long AccessHash { get; } = accessHash;
    public string FirstName { get; } = firstName;
    public string? LastName { get; } = lastName;
    public string PhoneNumber { get; } = phoneNumber;

    //public string PhoneNumber { get; }
    public string PhoneCodeHash { get; } = phoneCodeHash;

    //public string Code { get; } 
    public long UserId { get; } = userId;
}
