namespace MyTelegram.Domain.Events.Dialog;

public class DialogUpdatedEvent(RequestInfo requestInfo, long ownerUserId, Peer toPeer, int topMessageId, int pts, bool isNew, int? defaultHistoryTtl)
    : AggregateEvent<DialogAggregate, DialogId>, IHasRequestInfo
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long OwnerUserId { get; } = ownerUserId;
    public Peer ToPeer { get; } = toPeer;
    public int TopMessageId { get; } = topMessageId;
    public int Pts { get; } = pts;
    public bool IsNew { get; } = isNew;
    public int? DefaultHistoryTtl { get; } = defaultHistoryTtl;
}