namespace MyTelegram.Domain.Commands.Dialog;

public class UpdateDialogFolderCommand(DialogId aggregateId, RequestInfo requestInfo, int? folderId) : Command<DialogAggregate, DialogId, IExecutionResult>(aggregateId)
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public int? FolderId { get; } = folderId;
}