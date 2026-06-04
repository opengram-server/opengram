namespace MyTelegram.Domain.CommandHandlers.Temp;

public class StartEditPeerFoldersCommandHandler : CommandHandler<TempAggregate, TempId, StartEditPeerFoldersCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, StartEditPeerFoldersCommand command, CancellationToken cancellationToken)
    {
        aggregate.StartEditPeerFolders(command.RequestInfo,command.FolderPeers);

        return Task.CompletedTask;
    }
}