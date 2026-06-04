namespace MyTelegram.Domain.Sagas.Events;

public class SignInStartedSagaEvent(RequestInfo requestInfo) : RequestAggregateEvent2<SignInSaga, SignInSagaId>(requestInfo);
