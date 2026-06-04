namespace MyTelegram.Domain.CommandHandlers.Dialog;

public class SetDialogHistoryTTLCommandHandler : CommandHandler<DialogAggregate, DialogId, SetDialogHistoryTTLCommand>
{
    public override Task ExecuteAsync(DialogAggregate aggregate, SetDialogHistoryTTLCommand command, CancellationToken cancellationToken)
    {
        aggregate.SetHistoryTTL(command.RequestInfo, command.Peer, command.TtlPeriod);
        return Task.CompletedTask;
    }
}
