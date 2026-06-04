namespace MyTelegram;

/// <summary>
/// Quick reply item
/// </summary>
/// <param name="ShortcutId">Unique shortcut ID</param>
/// <param name="Title">Shortcut name</param>
/// <param name="TopMessageId">ID of the last message in the shortcut</param>
/// <param name="Count">Total number of messages in the shortcut</param>
/// <param name="ShouldCreateNewQuickReply"></param>
public record QuickReplyItem(int ShortcutId, string Title, int TopMessageId, int Count, bool ShouldCreateNewQuickReply);