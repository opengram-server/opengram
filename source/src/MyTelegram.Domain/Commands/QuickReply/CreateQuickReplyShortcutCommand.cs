using MyTelegram.Domain.Aggregates.QuickReply;

namespace MyTelegram.Domain.Commands.QuickReply;

public class CreateQuickReplyShortcutCommand(
    QuickReplyId aggregateId,
    RequestInfo requestInfo,
    int shortcutId,
    string shortcut)
    : Command<QuickReplyAggregate, QuickReplyId, IExecutionResult>(aggregateId)
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public int ShortcutId { get; } = shortcutId;
    public string Shortcut { get; } = shortcut;
}
