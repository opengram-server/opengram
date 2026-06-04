namespace MyTelegram.Domain.Events.Temp;

public class DeleteQuickReplyMessagesStartedEvent(RequestInfo requestInfo, int shortcutId, List<int> messageIds) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public int ShortcutId { get; } = shortcutId;
    public List<int> MessageIds { get; } = messageIds;
}