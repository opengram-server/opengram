namespace MyTelegram.Domain.Events.Messaging;

public class InboxMessageCreatedEvent(
    RequestInfo requestInfo,
    MessageItem inboxMessageItem,
    int senderMessageId,
    int? senderDefaultHistoryTTL = null)
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    public MessageItem InboxMessageItem { get; } = inboxMessageItem;
    public int SenderMessageId { get; } = senderMessageId;
    public int? SenderDefaultHistoryTTL { get; } = senderDefaultHistoryTTL;
}