namespace MyTelegram.Domain.Commands.Messaging;

public class UpdateInboxMessagePinnedCommand(
    MessageId aggregateId,
    RequestInfo requestInfo,
    bool pinned,
    bool pmOneSize,
    bool silent,
    int date)
    : Command<MessageAggregate, MessageId, IExecutionResult>(aggregateId), IHasRequestInfo
{
    public bool Pinned { get; } = pinned;
    public bool PmOneSize { get; } = pmOneSize;
    public bool Silent { get; } = silent;
    public int Date { get; } = date;

    public RequestInfo RequestInfo { get; } = requestInfo;
}