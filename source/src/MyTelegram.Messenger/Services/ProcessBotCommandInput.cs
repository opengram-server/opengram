namespace MyTelegram.Messenger.Services;

public class ProcessBotCommandInput(
    long botUserId,
    long senderPeerId,
    int senderMessageId,
    int messageId,
    string command,
    Peer replyToPeer,
    int updateId,
    bool isChannelPost = false,
    string? data = null,
    byte[]? media = null)
{
    //PeerType replyToPeerType,
    //long replyToPeerId,
    //ReplyToPeerType = replyToPeerType;
    //ReplyToPeerId = replyToPeerId;

    public long BotUserId { get; } = botUserId;
    public int SenderMessageId { get; } = senderMessageId;
    public string Command { get; } = command;
    public Peer ReplyToPeer { get; } = replyToPeer;
    public bool IsChannelPost { get; } = isChannelPost;
    public string? Data { get; } = data;
    public byte[]? Media { get; } = media;

    public int MessageId { get; } = messageId;

    //public long ReplyToPeerId { get; }
    //public PeerType ReplyToPeerType { get; }
    public long SenderPeerId { get; } = senderPeerId;
    public int UpdateId { get; set; } = updateId;
}