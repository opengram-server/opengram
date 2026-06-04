namespace MyTelegram.Domain.Sagas.Identities;

public class CreateUserSagaLocator : DefaultSagaLocator<CreateUserSaga, CreateUserSagaId>
{
    protected override CreateUserSagaId CreateSagaId(string requestId)
    {
        return new CreateUserSagaId(requestId);
    }
}