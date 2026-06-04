using MyTelegram.Domain.Sagas.Identities;

namespace MyTelegram.Domain.Sagas.Identities;

public class UserPasswordSyncSagaLocator : DefaultSagaLocator<UserPasswordSyncSaga, UserPasswordSyncSagaId>
{
    protected override UserPasswordSyncSagaId CreateSagaId(string requestId)
    {
        return new UserPasswordSyncSagaId(requestId);
    }
}
