namespace MyTelegram;

public class Draft(
    bool noWebpage,
    bool invertMedia,
    int? replyToMsgId,
    string message,
    int date,
    byte[]? entities = null,
    IList<IMessageEntity>? entities2 = null,
    IMessageMedia? media = null,
    int? topMsgId = null,
    long? effect = null,
    IInputMedia? media2 = null,
    IInputReplyTo? replyTo = null
)
{
    //bool? invertMedia,

    public string Message { get; init; } = message;
    public bool NoWebpage { get; init; } = noWebpage;
    public bool InvertMedia { get; } = invertMedia;
    public int? ReplyToMsgId { get; init; } = replyToMsgId;
    public int Date { get; init; } = date;
    public byte[]? Entities { get; init; } = entities;
    public IList<IMessageEntity>? Entities2 { get; } = entities2;
    public IMessageMedia? Media { get; } = media;
    public int? TopMsgId { get; init; } = topMsgId;
    public long? Effect { get; } = effect;
    public IInputMedia? Media2 { get; } = media2;
    public IInputReplyTo? ReplyTo { get; } = replyTo;
}