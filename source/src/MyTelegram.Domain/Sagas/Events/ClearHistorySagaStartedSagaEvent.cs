namespace MyTelegram.Domain.Sagas.Events;

public class ClearHistorySagaStartedSagaEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    bool revoke,
    Peer toPeer,
    string messageActionData,
    long randomId,
    int totalCountToBeDelete,
    int nextMaxId)
    : RequestAggregateEvent2<ClearHistorySaga, ClearHistorySagaId>(requestInfo)
{
    public string MessageActionData { get; } = messageActionData;
    public int NextMaxId { get; } = nextMaxId;
    public long OwnerPeerId { get; } = ownerPeerId;
    public long RandomId { get; } = randomId;
    public bool Revoke { get; } = revoke;
    public Peer ToPeer { get; } = toPeer;
    public int TotalCountToBeDelete { get; } = totalCountToBeDelete;
}
