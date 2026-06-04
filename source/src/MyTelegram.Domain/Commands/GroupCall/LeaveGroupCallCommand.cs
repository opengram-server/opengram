using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Commands.GroupCall;

public class LeaveGroupCallCommand(
    GroupCallId aggregateId,
    RequestInfo requestInfo,
    long peerId,
    int source,
    int date)
    : RequestCommand2<GroupCallAggregate, GroupCallId, IExecutionResult>(aggregateId, requestInfo)
{
    public long PeerId { get; } = peerId;
    public int Source { get; } = source;
    public int Date { get; } = date;

    // Use base ReqMsgId for SourceId to avoid duplicate command issues
    // Each request will have unique SourceId
}
