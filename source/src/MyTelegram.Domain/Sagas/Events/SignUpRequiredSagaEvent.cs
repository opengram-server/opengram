namespace MyTelegram.Domain.Sagas.Events;

public class SignUpRequiredSagaEvent(RequestInfo requestInfo)
    : RequestAggregateEvent2<SignInSaga, SignInSagaId>(requestInfo);
