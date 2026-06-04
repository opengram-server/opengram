namespace MyTelegram.Domain.Events.AppCode;

public class CheckSignUpCodeCompletedEvent(
    RequestInfo requestInfo,
    bool isCodeValid,
    long userId,
    long accessHash,
    string phoneNumber,
    string firstName,
    string? lastName)
    : RequestAggregateEvent2<AppCodeAggregate, AppCodeId>(requestInfo)
{
    //public AppCodeCheckCompletedEvent(long reqMsgId,
    //    bool isCodeValid,
    //    string errorMessage,
    //    Guid correlationId) : base(reqMsgId)
    //{
    //    IsCodeValid = isCodeValid;
    //    ErrorMessage = errorMessage;
    //    
    //}

    //public bool IsCodeValid { get; }
    //public string ErrorMessage { get; }
    //

    public long AccessHash { get; } = accessHash;
    public string FirstName { get; } = firstName;

    public bool IsCodeValid { get; } = isCodeValid;
    public string? LastName { get; } = lastName;
    public string PhoneNumber { get; } = phoneNumber;
    public long UserId { get; } = userId;
}