namespace MyTelegram.Domain.Aggregates.Dialog;

public class DialogSnapshot(
    long ownerId,
    int topMessage,
    int readInboxMaxId,
    int readOutboxMaxId,
    int unreadCount,
    Peer toPeer,
    bool unreadMark,
    bool pinned,
    int channelHistoryMinId,
    Draft? draft,
    int unreadMentionsCount,
    int? folderId
    )
    : ISnapshot
{
    public int ChannelHistoryMinId { get; } = channelHistoryMinId;

    public Draft? Draft { get; } = draft;
    public int UnreadMentionsCount { get; } = unreadMentionsCount;
    public int? FolderId { get; } = folderId;

    public long OwnerId { get; } = ownerId;

    public bool Pinned { get; } = pinned;
    public int ReadInboxMaxId { get; } = readInboxMaxId;
    public int ReadOutboxMaxId { get; } = readOutboxMaxId;
    public Peer ToPeer { get; } = toPeer;
    public int TopMessage { get; } = topMessage;
    public int UnreadCount { get; } = unreadCount;
    public bool UnreadMark { get; } = unreadMark;
}
