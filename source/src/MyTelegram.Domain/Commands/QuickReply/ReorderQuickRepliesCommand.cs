using MyTelegram.Domain.Aggregates.QuickReply;

namespace MyTelegram.Domain.Commands.QuickReply;

public class ReorderQuickRepliesCommand(
    QuickReplyId aggregateId,
    RequestInfo requestInfo,
    List<int> order)
    : Command<QuickReplyAggregate, QuickReplyId, IExecutionResult>(aggregateId)
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public List<int> Order { get; } = order;
}
