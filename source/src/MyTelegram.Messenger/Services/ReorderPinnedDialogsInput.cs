namespace MyTelegram.Messenger.Services;

public class ReorderPinnedDialogsInput(
    long selfUserId,
    List<Peer> orderedPeerList)
{
    public List<Peer> OrderedPeerList { get; } = orderedPeerList;

    public long SelfUserId { get; } = selfUserId;
}