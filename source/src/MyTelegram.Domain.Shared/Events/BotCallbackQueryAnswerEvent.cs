namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Event for answering a callback query via Bot API.
/// </summary>
public class BotCallbackQueryAnswerEvent
{
    public long BotUserId { get; set; }
    public long QueryId { get; set; }
    public string? Text { get; set; }
    public bool ShowAlert { get; set; }
    public string? Url { get; set; }
    public int CacheTime { get; set; }
    public long Timestamp { get; set; }
}
