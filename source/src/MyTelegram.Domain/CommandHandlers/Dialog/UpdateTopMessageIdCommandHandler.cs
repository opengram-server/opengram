namespace MyTelegram.Domain.CommandHandlers.Dialog;

public class UpdateTopMessageIdCommandHandler : CommandHandler<DialogAggregate, DialogId, UpdateTopMessageIdCommand>
{
    public override Task ExecuteAsync(DialogAggregate aggregate, UpdateTopMessageIdCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateTopMessageId(command.NewTopMessageId);

        return Task.CompletedTask;
    }
}