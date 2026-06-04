namespace MyTelegram.Domain.Commands.Dialog;

public class UpdateDialogFilterCommand(
    DialogFilterId aggregateId,
    RequestInfo requestInfo,
    long ownerUserId,
    int folderId,
    DialogFilter filter
    )
    : RequestCommand2<DialogFilterAggregate, DialogFilterId, IExecutionResult>(aggregateId, requestInfo)
{
    public long OwnerUserId { get; } = ownerUserId;
    public int FolderId { get; } = folderId;
    public DialogFilter Filter { get; } = filter;
}