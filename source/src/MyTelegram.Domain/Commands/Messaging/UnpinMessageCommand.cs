namespace MyTelegram.Domain.Commands.Messaging;

public class UnpinMessageCommand(MessageId aggregateId,
    RequestInfo requestInfo) : Command<MessageAggregate, MessageId, IExecutionResult>(aggregateId), IHasRequestInfo
{
    public RequestInfo RequestInfo { get; } = requestInfo;
}