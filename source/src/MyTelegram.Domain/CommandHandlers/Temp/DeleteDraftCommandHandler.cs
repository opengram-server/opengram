namespace MyTelegram.Domain.CommandHandlers.Temp;

public class DeleteDraftCommandHandler : CommandHandler<TempAggregate, TempId, DeleteDraftCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, DeleteDraftCommand command, CancellationToken cancellationToken)
    {
        aggregate.DeleteDraft(command.OwnerPeerId, command.ToPeer);

        return Task.CompletedTask;
    }
}