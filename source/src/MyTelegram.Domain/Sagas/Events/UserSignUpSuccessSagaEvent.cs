namespace MyTelegram.Domain.Sagas.Events;

public class UserSignUpSuccessSagaEvent(
    RequestInfo requestInfo,
    long userId,
    string phoneNumber)
    : RequestAggregateEvent2<UserSignUpSaga, UserSignUpSagaId>(requestInfo)
{
    public string PhoneNumber { get; } = phoneNumber;

    public long UserId { get; } = userId;
}