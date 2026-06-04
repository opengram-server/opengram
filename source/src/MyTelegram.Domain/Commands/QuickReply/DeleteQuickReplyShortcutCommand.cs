using MyTelegram.Domain.Aggregates.QuickReply;

namespace MyTelegram.Domain.Commands.QuickReply;

public class DeleteQuickReplyShortcutCommand(
    QuickReplyId aggregateId,
    RequestInfo requestInfo,
    int shortcutId)
    : Command<QuickReplyAggregate, QuickReplyId, IExecutionResult>(aggregateId)
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public int ShortcutId { get; } = shortcutId;
}
