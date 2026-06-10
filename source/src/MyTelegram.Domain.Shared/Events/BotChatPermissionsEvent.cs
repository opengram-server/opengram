namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Event for setting default chat permissions via Bot API.
/// </summary>
public class BotChatPermissionsEvent
{
    public long BotUserId { get; set; }
    public long ChatId { get; set; }
    public string PermissionsJson { get; set; } = "{}";
    public bool UseIndependentChatPermissions { get; set; }
    public long Timestamp { get; set; }
}
