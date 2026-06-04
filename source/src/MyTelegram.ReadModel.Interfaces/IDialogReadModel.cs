namespace MyTelegram.ReadModel.Interfaces;

public interface IDialogReadModel : IReadModel
{
    int ChannelHistoryMinId { get; }
    DateTime CreationTime { get; }

    Draft? Draft { get; }
    string Id { get; }

    int MaxSendOutMessageId { get; }
    PeerNotifySettings? NotifySettings { get; }

    long OwnerId { get; }

    //string TopMessageBoxId { get; }
    bool Pinned { get; }
    int PinnedMsgId { get; }
    int PinnedOrder { get; }
    int Pts { get; }

    /// <summary>
    /// The maximum ID of a received message that has been read (messages.ReadHistory)
    /// </summary>
    int ReadInboxMaxId { get; }

    /// <summary>
    /// The max ID of the message we sent that was read by the other party
    /// </summary>
    int ReadOutboxMaxId { get; }
    long ToPeerId { get; }
    PeerType ToPeerType { get; }
    int TopMessage { get; set; }
    int UnreadCount { get; }
    bool IsDeleted { get; }
    int? TtlPeriod { get; set; }
    int UnreadMentionsCount { get; }
    int UnreadReactionsCount { get; }
    int? FolderId { get; }
    bool ViewForumAsMessages { get; }
}