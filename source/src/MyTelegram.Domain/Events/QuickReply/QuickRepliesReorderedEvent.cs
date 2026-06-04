using MyTelegram.Domain.Aggregates.QuickReply;

namespace MyTelegram.Domain.Events.QuickReply;

public class QuickRepliesReorderedEvent(
    RequestInfo requestInfo,
    long ownerUserId,
    List<int> order)
    : RequestAggregateEvent2<QuickReplyAggregate, QuickReplyId>(requestInfo)
{
    public long OwnerUserId { get; } = ownerUserId;
    public List<int> Order { get; } = order;
}
