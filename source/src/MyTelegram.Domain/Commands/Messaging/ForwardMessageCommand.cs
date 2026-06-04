namespace MyTelegram.Domain.Commands.Messaging;

public class ForwardMessageCommand(
    MessageId aggregateId,
    RequestInfo requestInfo,
    long randomId)
    : Command<MessageAggregate, MessageId, IExecutionResult>(aggregateId), IHasRequestInfo
{
    public long RandomId { get; } = randomId;

    public RequestInfo RequestInfo { get; } = requestInfo;
}