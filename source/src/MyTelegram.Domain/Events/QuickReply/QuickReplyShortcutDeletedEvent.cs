using MyTelegram.Domain.Aggregates.QuickReply;

namespace MyTelegram.Domain.Events.QuickReply;

public class QuickReplyShortcutDeletedEvent(
    RequestInfo requestInfo,
    long ownerUserId,
    int shortcutId)
    : RequestAggregateEvent2<QuickReplyAggregate, QuickReplyId>(requestInfo)
{
    public long OwnerUserId { get; } = ownerUserId;
    public int ShortcutId { get; } = shortcutId;
}
