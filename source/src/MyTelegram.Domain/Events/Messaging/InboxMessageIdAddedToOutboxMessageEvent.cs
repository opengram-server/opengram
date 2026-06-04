namespace MyTelegram.Domain.Events.Messaging;

public class InboxMessageIdAddedToOutboxMessageEvent(InboxItem inboxItem) : AggregateEvent<MessageAggregate, MessageId>
{
    public InboxItem InboxItem { get; } = inboxItem;
}