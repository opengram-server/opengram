namespace MyTelegram.Domain.CommandHandlers.Dialog;

public class UpdateReadInboxMaxIdCommandHandler : CommandHandler<DialogAggregate, DialogId, UpdateReadInboxMaxIdCommand>
{
    public override Task ExecuteAsync(DialogAggregate aggregate, UpdateReadInboxMaxIdCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateReadInboxMaxId(command.RequestInfo, command.MaxId, command.SenderUserId, command.SenderMessageId, command.UnreadCount);

        return Task.CompletedTask;
    }
}