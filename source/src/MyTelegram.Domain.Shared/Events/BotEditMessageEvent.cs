namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Event for editing a message via Bot API.
/// </summary>
public class BotEditMessageEvent
{
    public long BotUserId { get; set; }
    public long ChatId { get; set; }
    public int MessageId { get; set; }
    public string? InlineMessageId { get; set; }
    public string? Text { get; set; }
    public string? ParseMode { get; set; }
    public string? ReplyMarkupJson { get; set; }
    public bool EditText { get; set; }
    public bool EditReplyMarkup { get; set; }
    public long Timestamp { get; set; }
}
