using MyTelegram.Domain.Aggregates.QuickReply;

namespace MyTelegram.Domain.Events.QuickReply;

public class QuickReplyShortcutUpdatedEvent(
    RequestInfo requestInfo,
    long ownerUserId,
    int shortcutId,
    string newShortcut)
    : RequestAggregateEvent2<QuickReplyAggregate, QuickReplyId>(requestInfo)
{
    public long OwnerUserId { get; } = ownerUserId;
    public int ShortcutId { get; } = shortcutId;
    public string NewShortcut { get; } = newShortcut;
}
