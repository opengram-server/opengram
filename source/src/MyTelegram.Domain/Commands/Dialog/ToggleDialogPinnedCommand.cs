namespace MyTelegram.Domain.Commands.Dialog;

public class ToggleDialogPinnedCommand(
    DialogId aggregateId,
    RequestInfo requestInfo,
    bool pinned)
    : RequestCommand2<DialogAggregate, DialogId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool Pinned { get; } = pinned;
}
