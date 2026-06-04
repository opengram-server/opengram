using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Events.GroupCall;

public class GroupCallParticipantJoinedEvent(
    RequestInfo requestInfo,
    long callId,
    long peerId,
    PeerType peerType,
    int source,
    bool muted,
    bool videoStopped,
    string? @params,
    int date)
    : RequestAggregateEvent2<GroupCallAggregate, GroupCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public long PeerId { get; } = peerId;
    public PeerType PeerType { get; } = peerType;
    public int Source { get; } = source; // SSRC
    public bool Muted { get; } = muted;
    public bool VideoStopped { get; } = videoStopped;
    public string? Params { get; } = @params;
    public int Date { get; } = date;
}
