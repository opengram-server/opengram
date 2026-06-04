namespace MyTelegram.Domain.Sagas.Identities;

public class EditBannedSagaLocator : DefaultSagaLocator<EditBannedSaga, EditBannedSagaId>
{
    protected override EditBannedSagaId CreateSagaId(string requestId)
    {
        return new EditBannedSagaId(requestId);
    }
}
