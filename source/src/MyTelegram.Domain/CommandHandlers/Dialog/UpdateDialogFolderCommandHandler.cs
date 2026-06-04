namespace MyTelegram.Domain.CommandHandlers.Dialog;

public class UpdateDialogFolderCommandHandler : CommandHandler<DialogAggregate, DialogId, UpdateDialogFolderCommand>
{
    public override Task ExecuteAsync(DialogAggregate aggregate,
        UpdateDialogFolderCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.UpdateDialogFolder(command.RequestInfo, command.FolderId);
        return Task.CompletedTask;
    }
}