namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Event for approving/declining chat join requests via Bot API.
/// </summary>
public class BotChatJoinRequestEvent
{
    public long BotUserId { get; set; }
    public long ChatId { get; set; }
    public long UserId { get; set; }
    
    /// <summary>Approve or Decline</summary>
    public string Action { get; set; } = "";
    
    public long Timestamp { get; set; }
}
