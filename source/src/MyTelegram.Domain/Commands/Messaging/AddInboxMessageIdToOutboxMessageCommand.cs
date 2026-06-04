namespace MyTelegram.Domain.Commands.Messaging;

public class AddInboxMessageIdToOutboxMessageCommand(
    MessageId aggregateId,
    RequestInfo requestInfo,
    long inboxOwnerPeerId,
    int inboxMessageId)
    : RequestCommand2<MessageAggregate, MessageId,
        IExecutionResult>(aggregateId, requestInfo)
{
    public int InboxMessageId { get; } = inboxMessageId;

    public long InboxOwnerPeerId { get; } = inboxOwnerPeerId;
}