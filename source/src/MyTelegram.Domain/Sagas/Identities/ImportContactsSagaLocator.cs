namespace MyTelegram.Domain.Sagas.Identities;

public class ImportContactsSagaLocator : DefaultSagaLocator<ImportContactsSaga, ImportContactsSagaId>
{
    protected override ImportContactsSagaId CreateSagaId(string requestId)
    {
        return new ImportContactsSagaId(requestId);
    }
}