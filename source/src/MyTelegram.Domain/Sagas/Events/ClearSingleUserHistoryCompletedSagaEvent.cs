namespace MyTelegram.Domain.Sagas.Events;

public class
    ClearSingleUserHistoryCompletedSagaEvent(
        RequestInfo requestInfo,
        long selfAuthKeyId,
        int nextMaxId,
        bool isSelf,
        PeerType toPeerType,
        DeletedBoxItem deletedBoxItem)
    : RequestAggregateEvent2<ClearHistorySaga, ClearHistorySagaId>(requestInfo)
{
    public DeletedBoxItem DeletedBoxItem { get; } = deletedBoxItem;
    public bool IsSelf { get; } = isSelf;
    public int NextMaxId { get; } = nextMaxId;

    public long SelfAuthKeyId { get; } = selfAuthKeyId;
    public PeerType ToPeerType { get; } = toPeerType;
}
