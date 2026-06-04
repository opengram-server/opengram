namespace MyTelegram.Domain.Events.Messaging;

public class OutboxMessageCreatedEvent(
    RequestInfo requestInfo,
    MessageItem outboxMessageItem,
    List<long>? mentionedUserIds,
    List<ReplyToMsgItem>? replyToMsgItems,
    bool clearDraft,
    int groupItemCount,
    long? linkedChannelId,
    List<long>? chatMembers)
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    public MessageItem OutboxMessageItem { get; } = outboxMessageItem;
    public List<long>? MentionedUserIds { get; } = mentionedUserIds;

    public List<ReplyToMsgItem>? ReplyToMsgItems { get; } = replyToMsgItems;

    public bool ClearDraft { get; } = clearDraft;
    public int GroupItemCount { get; } = groupItemCount;
    public long? LinkedChannelId { get; } = linkedChannelId;
    public List<long>? ChatMembers { get; } = chatMembers;
}