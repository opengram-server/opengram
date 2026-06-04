namespace MyTelegram.Domain.Commands.Messaging;

public class UpdateMessagePinnedCommand(MessageId aggregateId,
    RequestInfo requestInfo,bool pinned) : Command<MessageAggregate, MessageId, IExecutionResult>(aggregateId), IHasRequestInfo
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public bool Pinned { get; } = pinned;
}