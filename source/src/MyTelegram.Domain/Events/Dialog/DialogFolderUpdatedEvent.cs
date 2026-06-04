namespace MyTelegram.Domain.Events.Dialog;

public class DialogFolderUpdatedEvent(RequestInfo requestInfo, long ownerUserId, Peer toPeer, int? folderId) : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo), IHasRequestInfo
{
    public long OwnerUserId { get; } = ownerUserId;
    public Peer ToPeer { get; } = toPeer;
    public int? FolderId { get; } = folderId;
}