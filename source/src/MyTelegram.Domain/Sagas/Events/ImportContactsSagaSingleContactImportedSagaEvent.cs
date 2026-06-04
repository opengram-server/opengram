namespace MyTelegram.Domain.Sagas.Events;

public class ImportContactsSagaSingleContactImportedSagaEvent(PhoneContact phoneContact)
    : AggregateEvent<ImportContactsSaga, ImportContactsSagaId>
{
    public PhoneContact PhoneContact { get; } = phoneContact;
}
