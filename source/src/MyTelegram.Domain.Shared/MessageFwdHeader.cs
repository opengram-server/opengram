namespace MyTelegram;

public class MessageFwdHeader
{
    public MessageFwdHeader(bool imported,
        bool savedOut,
        Peer? fromId,
        string? fromName,
        int? channelPost,
        string? postAuthor,
        int date,
        Peer? savedFromPeer,
        int? savedFromMsgId,
        Peer? savedFromId,
        string? savedFromName,
        int? savedDate,
        string? psaType,
        bool forwardFromLinkedChannel)
    {
        ChannelPost = channelPost;
        Date = date;
        Imported = imported;
        SavedOut = savedOut;
        FromId = fromId;
        FromName = fromName;
        PostAuthor = postAuthor;
        SavedFromMsgId = savedFromMsgId;
        SavedFromId = savedFromId;
        SavedFromName = savedFromName;
        SavedDate = savedDate;
        PsaType = psaType;
        ForwardFromLinkedChannel = forwardFromLinkedChannel;
        SavedFromPeer = savedFromPeer;
    }

    public MessageFwdHeader()
    {
        
    }

    /// <summary>
    ///     ID of the channel message that was forwarded
    /// </summary>
    public int? ChannelPost { get; set; }

    /// <summary>
    ///     When was the message originally sent
    /// </summary>
    public int Date { get; set; }

    public bool Imported { get; set; }
    public bool SavedOut { get; set; }

    /// <summary>
    ///     The ID of the user that originally sent the message
    /// </summary>
    public Peer? FromId { get; set; }

    /// <summary>
    ///     The name of the user that originally sent the message
    /// </summary>
    public string? FromName { get; set; }

    /// <summary>
    ///     For channels and if signatures are enabled, author of the channel message
    /// </summary>
    public string? PostAuthor { get; set; }

    /// <summary>
    ///     Only for messages forwarded to the current user (inputPeerSelf), ID of the message that was forwarded from the
    ///     original user/channel
    /// </summary>
    public int? SavedFromMsgId { get; set; }

    public Peer? SavedFromId { get; set; }
    public string? SavedFromName { get; set; }
    public int? SavedDate { get; set; }
    public string? PsaType { get; set; }
    public bool ForwardFromLinkedChannel { get; set; }

    /// <summary>
    /// Only for messages forwarded to the current user (inputPeerSelf), full info about the user/channel that originally
    /// sent the message
    /// </summary>
    public Peer? SavedFromPeer { get; set; }
}