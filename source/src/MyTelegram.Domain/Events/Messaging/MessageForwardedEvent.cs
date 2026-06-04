namespace MyTelegram.Domain.Events.Messaging;

public class MessageForwardedEvent(RequestInfo requestInfo, long randomId, MessageItem originalMessageItem)
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    public long RandomId { get; } = randomId;
    public MessageItem OriginalMessageItem { get; } = originalMessageItem;
}