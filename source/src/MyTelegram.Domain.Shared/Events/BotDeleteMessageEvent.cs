namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Event for deleting a message via Bot API.
/// </summary>
public class BotDeleteMessageEvent
{
    public long BotUserId { get; set; }
    public long ChatId { get; set; }
    public int MessageId { get; set; }
    public long Timestamp { get; set; }
}
