namespace MyTelegram.Domain.Commands.Messaging;

public class AddInboxItemsToOutboxMessageCommand(
    MessageId aggregateId,
    RequestInfo requestInfo,
    List<InboxItem> inboxItems)
    : RequestCommand2<MessageAggregate, MessageId, IExecutionResult>(aggregateId, requestInfo)
{
    public List<InboxItem> InboxItems { get; } = inboxItems;
}