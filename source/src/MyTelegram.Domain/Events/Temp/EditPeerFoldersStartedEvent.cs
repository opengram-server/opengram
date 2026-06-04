namespace MyTelegram.Domain.Events.Temp;

public class EditPeerFoldersStartedEvent(RequestInfo requestInfo, IEnumerable<IInputFolderPeer> folderPeers) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public IEnumerable<IInputFolderPeer> FolderPeers { get; } = folderPeers;
}