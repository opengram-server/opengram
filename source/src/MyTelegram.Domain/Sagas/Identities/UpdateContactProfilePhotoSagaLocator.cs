namespace MyTelegram.Domain.Sagas.Identities;

public class UpdateContactProfilePhotoSagaLocator : DefaultSagaLocator<UpdateContactProfilePhotoSaga, UpdateContactProfilePhotoSagaId>
{
    protected override UpdateContactProfilePhotoSagaId CreateSagaId(string requestId)
    {
        return new UpdateContactProfilePhotoSagaId(requestId);
    }
}