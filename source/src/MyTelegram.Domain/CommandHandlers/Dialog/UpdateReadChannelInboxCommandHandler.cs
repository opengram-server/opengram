namespace MyTelegram.Domain.CommandHandlers.Dialog;

public class
    UpdateReadChannelInboxCommandHandler : CommandHandler<DialogAggregate, DialogId, UpdateReadChannelInboxCommand>
{
    public override Task ExecuteAsync(DialogAggregate aggregate,
        UpdateReadChannelInboxCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.UpdateReadChannelInbox(command.RequestInfo, command.MessageSenderUserId, command.MaxId);
        return Task.CompletedTask;
    }
}