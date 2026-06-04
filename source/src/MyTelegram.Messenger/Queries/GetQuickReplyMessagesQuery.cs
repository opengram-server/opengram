using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Messenger.Queries;

public class GetQuickReplyMessagesQuery(long userId, int shortcutId, List<int> messageIds) : IQuery<IReadOnlyCollection<IMessageReadModel>>
{
    public long UserId { get; } = userId;
    public int ShortcutId { get; } = shortcutId;
    public List<int> MessageIds { get; } = messageIds;
}
