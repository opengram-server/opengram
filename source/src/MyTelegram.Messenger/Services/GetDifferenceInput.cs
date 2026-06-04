namespace MyTelegram.Messenger.Services;

public class GetDifferenceInput(
    long selfUserId,
    long ownerPeerId,
    int pts,
    int limit,
    List<int>? messageIds,
    List<long>? users = null,
    List<long>? chats = null)
{
    public int Limit { get; } = limit;
    public List<int>? MessageIds { get; } = messageIds;
    public List<long>? Users { get; } = users;
    public List<long>? Chats { get; } = chats;

    public long OwnerPeerId { get; } = ownerPeerId;
    public int Pts { get; } = pts;
    public long SelfUserId { get; } = selfUserId;
}