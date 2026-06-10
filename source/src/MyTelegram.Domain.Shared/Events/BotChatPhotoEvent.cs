namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Event for setting/deleting chat photo via Bot API.
/// </summary>
public class BotChatPhotoEvent
{
    public long BotUserId { get; set; }
    public long ChatId { get; set; }
    
    /// <summary>Set or Delete</summary>
    public string Action { get; set; } = "";
    
    /// <summary>Base64-encoded photo data (for Set action)</summary>
    public string? PhotoBase64 { get; set; }
    
    public long Timestamp { get; set; }
}
