namespace MyTelegram.Domain.Commands.Dialog;

public class MarkDialogAsUnreadCommand(
    DialogId aggregateId,
    RequestInfo requestInfo,
    bool unread)
    : RequestCommand2<DialogAggregate, DialogId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool Unread { get; } = unread;
}

