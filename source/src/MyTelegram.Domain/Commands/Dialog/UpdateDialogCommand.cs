namespace MyTelegram.Domain.Commands.Dialog;

public class UpdateDialogCommand(
    DialogId aggregateId,
    RequestInfo requestInfo,
    long ownerUserId,
    Peer toPeer,
    int topMessageId,
    int pts,
    int? defaultHistoryTtl
    )
    : Command<DialogAggregate, DialogId, IExecutionResult>(aggregateId)
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long OwnerUserId { get; } = ownerUserId;
    public Peer ToPeer { get; } = toPeer;
    public int TopMessageId { get; } = topMessageId;
    public int Pts { get; } = pts;
    public int? DefaultHistoryTtl { get; } = defaultHistoryTtl;
}