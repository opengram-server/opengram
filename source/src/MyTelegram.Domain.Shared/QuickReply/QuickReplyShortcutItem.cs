namespace MyTelegram.Domain.Shared.QuickReply;

/// <summary>
/// Internal state item for Quick Reply shortcuts
/// </summary>
public class QuickReplyShortcutItem
{
    public int ShortcutId { get; set; }
    public string Shortcut { get; set; } = string.Empty;
}
