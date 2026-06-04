namespace MyTelegram.Domain.Events.Messaging;

public class ReplyToMessageEvent(
    RequestInfo requestInfo,
    int senderMessageId,
    IReadOnlyList<InboxItem>? inboxItems)
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    public int SenderMessageId { get; } = senderMessageId;
    public IReadOnlyList<InboxItem>? InboxItems { get; } = inboxItems;
}