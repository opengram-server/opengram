namespace MyTelegram.Domain.Events.Dialog;

public class HistoryClearedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    int historyMinId,
    bool revoke,
    Peer toPeer,
    string messageActionData,
    long randomId,
    List<int> messageIdListToBeDelete,
    int nextMaxId)
    : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    public int HistoryMinId { get; } = historyMinId;
    public string MessageActionData { get; } = messageActionData;
    public List<int> MessageIdListToBeDelete { get; } = messageIdListToBeDelete;
    public int NextMaxId { get; } = nextMaxId;
    public long OwnerPeerId { get; } = ownerPeerId;
    public long RandomId { get; } = randomId;
    public bool Revoke { get; } = revoke;
    public Peer ToPeer { get; } = toPeer;
}