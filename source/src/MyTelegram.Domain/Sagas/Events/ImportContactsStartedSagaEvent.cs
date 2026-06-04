namespace MyTelegram.Domain.Sagas.Events;

public class ImportContactsStartedSagaEvent(
    RequestInfo requestInfo,
    int count) : RequestAggregateEvent2<ImportContactsSaga, ImportContactsSagaId>(requestInfo)
{
    public int Count { get; } = count;
}
