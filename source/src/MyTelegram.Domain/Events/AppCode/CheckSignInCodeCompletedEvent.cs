namespace MyTelegram.Domain.Events.AppCode;

public class CheckSignInCodeCompletedEvent(
    RequestInfo requestInfo,
    bool isCodeValid,
    long userId)
    : RequestAggregateEvent2<AppCodeAggregate, AppCodeId>(requestInfo)
{
    //,
    //long accessHash,
    //string phoneNumber,
    //string firstName,
    //string lastName,
    //Guid correlationId
    //AccessHash = accessHash;
    //PhoneNumber = phoneNumber;
    //FirstName = firstName;
    //LastName = lastName;
    //

    public bool IsCodeValid { get; } = isCodeValid;

    public long UserId { get; } = userId;

    //public long AccessHash { get; }
    //public string PhoneNumber { get; }
    //public string FirstName { get; }
    //public string LastName { get; }
    //
}