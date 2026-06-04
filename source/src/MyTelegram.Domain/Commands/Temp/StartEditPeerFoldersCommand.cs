namespace MyTelegram.Domain.Commands.Temp;

public class StartEditPeerFoldersCommand(
    TempId aggregateId,
    RequestInfo requestInfo,
    IEnumerable<IInputFolderPeer> folderPeers) : RequestCommand2<TempAggregate, TempId, IExecutionResult>(aggregateId, requestInfo)
{
    public IEnumerable<IInputFolderPeer> FolderPeers { get; } = folderPeers;
}