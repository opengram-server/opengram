namespace MyTelegram.Domain.CommandHandlers.Dialog;

public class UpdateReadOutboxMaxIdCommandHandler : CommandHandler<DialogAggregate, DialogId, UpdateReadOutboxMaxIdCommand>
{
    public override Task ExecuteAsync(DialogAggregate aggregate, UpdateReadOutboxMaxIdCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateReadOutboxMaxId(command.RequestInfo, command.MaxId);

        return Task.CompletedTask;
    }
}