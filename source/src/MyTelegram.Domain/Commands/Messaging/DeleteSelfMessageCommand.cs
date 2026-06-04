namespace MyTelegram.Domain.Commands.Messaging;

public class DeleteSelfMessageCommand(
    MessageId aggregateId,
    RequestInfo requestInfo,
    int messageId)
    : RequestCommand2<MessageAggregate, MessageId, IExecutionResult>(aggregateId, requestInfo)
{
    public int MessageId { get; } = messageId;
}