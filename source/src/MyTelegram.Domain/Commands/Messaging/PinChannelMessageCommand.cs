namespace MyTelegram.Domain.Commands.Messaging;

public class PinChannelMessageCommand(MessageId aggregateId, RequestInfo requestInfo)
    : Command<MessageAggregate, MessageId, IExecutionResult>(aggregateId), IHasRequestInfo
{
    public RequestInfo RequestInfo { get; } = requestInfo;
}