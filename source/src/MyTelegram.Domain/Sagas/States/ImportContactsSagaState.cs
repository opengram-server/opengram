namespace MyTelegram.Domain.Sagas.States;

public class
    ImportContactsSagaState : AggregateState<ImportContactsSaga, ImportContactsSagaId, ImportContactsSagaState>,
        IApply<ImportContactsStartedSagaEvent>,
        IApply<ImportContactsSagaSingleContactImportedSagaEvent>
{
    public List<PhoneContact> PhoneContacts { get; private set; } = null!;
    public RequestInfo RequestInfo { get; private set; } = null!;
    public int TotalCount { get; private set; }

    public void Apply(ImportContactsSagaSingleContactImportedSagaEvent aggregateEvent)
    {
        PhoneContacts.Add(aggregateEvent.PhoneContact);
    }

    public void Apply(ImportContactsStartedSagaEvent aggregateEvent)
    {
        RequestInfo=aggregateEvent.RequestInfo;
        TotalCount = aggregateEvent.Count;
        PhoneContacts = new List<PhoneContact>();
    }

    public bool IsCompleted()
    {
        return TotalCount == PhoneContacts.Count;
    }
}
