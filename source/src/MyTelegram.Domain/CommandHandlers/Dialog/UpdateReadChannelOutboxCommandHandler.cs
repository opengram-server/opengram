namespace MyTelegram.Domain.CommandHandlers.Dialog;

public class
    UpdateReadChannelOutboxCommandHandler : CommandHandler<DialogAggregate, DialogId, UpdateReadChannelOutboxCommand>
{
    public override Task ExecuteAsync(DialogAggregate aggregate,
        UpdateReadChannelOutboxCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.UpdateReadChannelOutbox(command.RequestInfo, command.MaxId);
        return Task.CompletedTask;
    }
}