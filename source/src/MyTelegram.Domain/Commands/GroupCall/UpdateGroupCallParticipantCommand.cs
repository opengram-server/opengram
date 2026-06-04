using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Commands.GroupCall;

public class UpdateGroupCallParticipantCommand(
    GroupCallId aggregateId,
    RequestInfo requestInfo,
    long peerId,
    bool? muted,
    bool mutedByAdmin,
    int? volume,
    bool? raiseHand,
    bool? videoStopped,
    int date)
    : RequestCommand2<GroupCallAggregate, GroupCallId, IExecutionResult>(aggregateId, requestInfo)
{
    public long PeerId { get; } = peerId;
    public bool? Muted { get; } = muted;
    public bool MutedByAdmin { get; } = mutedByAdmin;
    public int? Volume { get; } = volume;
    public bool? RaiseHand { get; } = raiseHand;
    public bool? VideoStopped { get; } = videoStopped;
    public int Date { get; } = date;

    // Use base ReqMsgId for SourceId to avoid duplicate command issues
    // Each request will have unique SourceId
}
