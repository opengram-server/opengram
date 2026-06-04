namespace MyTelegram.Domain.CommandHandlers.Dialog;

public class UpdateDialogCommandHandler : CommandHandler<DialogAggregate, DialogId, UpdateDialogCommand>
{
    public override Task ExecuteAsync(DialogAggregate aggregate, UpdateDialogCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateDialog(command.RequestInfo, command.OwnerUserId, command.ToPeer, command.TopMessageId, command.Pts, command.DefaultHistoryTtl);

        return Task.CompletedTask;
    }
}