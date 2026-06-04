namespace MyTelegram.Domain.Commands.Dialog;

public class UpdateReadChannelInboxCommand(
    DialogId aggregateId,
    RequestInfo requestInfo,
    long messageSenderUserId,
    int maxId)
    : Command<DialogAggregate, DialogId, IExecutionResult>(aggregateId), IHasRequestInfo
{
    public long MessageSenderUserId { get; } = messageSenderUserId;
    public int MaxId { get; } = maxId;

    public RequestInfo RequestInfo { get; } = requestInfo;
}