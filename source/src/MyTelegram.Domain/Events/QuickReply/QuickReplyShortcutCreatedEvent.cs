using MyTelegram.Domain.Aggregates.QuickReply;

namespace MyTelegram.Domain.Events.QuickReply;

public class QuickReplyShortcutCreatedEvent(
    RequestInfo requestInfo,
    long ownerUserId,
    int shortcutId,
    string shortcut)
    : RequestAggregateEvent2<QuickReplyAggregate, QuickReplyId>(requestInfo)
{
    public long OwnerUserId { get; } = ownerUserId;
    public int ShortcutId { get; } = shortcutId;
    public string Shortcut { get; } = shortcut;
}
