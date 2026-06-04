namespace MyTelegram.Domain.Commands.Messaging;

public class CheckMessageViewLogCommand(
    MessageViewLogId aggregateId,
    RequestInfo requestInfo,
    int messageId)
    : RequestCommand2<MessageViewLogAggregate, MessageViewLogId, IExecutionResult>(aggregateId, requestInfo)
{
    public int MessageId { get; } = messageId;
}
