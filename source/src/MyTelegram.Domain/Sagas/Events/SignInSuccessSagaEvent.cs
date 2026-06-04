namespace MyTelegram.Domain.Sagas.Events;

public class SignInSuccessSagaEvent(
    RequestInfo requestInfo,
    long tempAuthKeyId,
    long permAuthKeyId,
    long userId,
    long accessHash,
    bool signUpRequired,
    string phoneNumber,
    string firstName,
    string? lastName,
    bool hasPassword)
    : RequestAggregateEvent2<SignInSaga, SignInSagaId>(requestInfo)
{
    public string FirstName { get; } = firstName;
    public bool HasPassword { get; } = hasPassword;
    public string? LastName { get; } = lastName;
    public long PermAuthKeyId { get; } = permAuthKeyId;
    public string PhoneNumber { get; } = phoneNumber;

    public bool SignUpRequired { get; } = signUpRequired;
    public long TempAuthKeyId { get; } = tempAuthKeyId;
    public long UserId { get; } = userId;
    public long AccessHash { get; } = accessHash;
}
