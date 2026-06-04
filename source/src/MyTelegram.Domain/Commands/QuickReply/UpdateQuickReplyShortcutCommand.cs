using MyTelegram.Domain.Aggregates.QuickReply;

namespace MyTelegram.Domain.Commands.QuickReply;

public class UpdateQuickReplyShortcutCommand(
    QuickReplyId aggregateId,
    RequestInfo requestInfo,
    int shortcutId,
    string newShortcut)
    : Command<QuickReplyAggregate, QuickReplyId, IExecutionResult>(aggregateId)
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public int ShortcutId { get; } = shortcutId;
    public string NewShortcut { get; } = newShortcut;
}
