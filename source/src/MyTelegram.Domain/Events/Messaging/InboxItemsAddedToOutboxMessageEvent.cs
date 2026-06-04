namespace MyTelegram.Domain.Events.Messaging;

public class InboxItemsAddedToOutboxMessageEvent(List<InboxItem> inboxItems)
    : AggregateEvent<MessageAggregate, MessageId>
{
    public List<InboxItem> InboxItems { get; } = inboxItems;
}