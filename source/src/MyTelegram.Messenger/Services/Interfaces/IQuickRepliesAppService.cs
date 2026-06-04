using MyTelegram.Schema.Messages;

namespace MyTelegram.Messenger.Services.Impl;

public interface IQuickRepliesAppService
{
    Task<IQuickReplies> GetQuickRepliesAsync(long userId, long hash);
    Task<IMessages> GetQuickReplyMessagesAsync(long userId, int shortcutId, List<int>? messageIds = null, long hash = 0);
    Task<int> CreateQuickReplyShortcutAsync(long userId, string shortcut, int shortcutId = 0);
    Task EditQuickReplyShortcutAsync(long userId, int shortcutId, string newShortcut);
    Task DeleteQuickReplyShortcutAsync(long userId, int shortcutId);
    Task<List<long>> SendQuickReplyMessagesAsync(long userId, long peerId, int shortcutId);
    Task ReorderQuickRepliesAsync(long userId, List<int> order);
    Task DeleteQuickReplyMessagesAsync(long userId, int shortcutId, List<int> messageIds);
    Task<bool> CheckQuickReplyShortcutAsync(long userId, string shortcut);
}
