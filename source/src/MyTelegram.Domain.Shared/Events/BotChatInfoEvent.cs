namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Event for updating chat title/description via Bot API.
/// </summary>
public class BotChatInfoEvent
{
    public long BotUserId { get; set; }
    public long ChatId { get; set; }
    
    /// <summary>SetTitle or SetDescription</summary>
    public string Action { get; set; } = "";
    
    public string? Title { get; set; }
    public string? Description { get; set; }
    
    public long Timestamp { get; set; }
}
