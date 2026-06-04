using MyTelegram.Domain.Shared.QuickReply;

namespace MyTelegram.ReadModel.Interfaces;

public interface IQuickReplyReadModel : IReadModel
{
    long UserId { get; }
    List<QuickReplyShortcutItem> Shortcuts { get; }
}
