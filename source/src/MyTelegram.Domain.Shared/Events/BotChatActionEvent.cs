namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Event for sending a chat action (typing indicator) via Bot API.
/// </summary>
public class BotChatActionEvent
{
    public long BotUserId { get; set; }
    public long ChatId { get; set; }
    public string Action { get; set; } = "typing";
    public int? MessageThreadId { get; set; }
    public long Timestamp { get; set; }
}
