namespace MyTelegram.Domain.Events.Dialog;

public class DeleteUserMessagesStartedEvent(
    RequestInfo requestInfo,
    bool revoke,
    long toUserId,
    List<int> messageIds,
    bool isClearHistory)
    : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    public bool Revoke { get; } = revoke;
    public long ToUserId { get; } = toUserId;
    public List<int> MessageIds { get; } = messageIds;
    public bool IsClearHistory { get; } = isClearHistory;
}