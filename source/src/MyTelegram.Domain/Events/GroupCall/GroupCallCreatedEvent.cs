using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Events.GroupCall;

public class GroupCallCreatedEvent(
    RequestInfo requestInfo,
    long callId,
    long accessHash,
    long peerId,
    PeerType peerType,
    string? title,
    bool rtmpStream,
    int? streamDcId,
    int? scheduleDate,
    int randomId,
    int date)
    : RequestAggregateEvent2<GroupCallAggregate, GroupCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public long AccessHash { get; } = accessHash;
    public long PeerId { get; } = peerId;
    public PeerType PeerType { get; } = peerType;
    public string? Title { get; } = title;
    public bool RtmpStream { get; } = rtmpStream;
    public int? StreamDcId { get; } = streamDcId;
    public int? ScheduleDate { get; } = scheduleDate;
    public int RandomId { get; } = randomId;
    public int Date { get; } = date;
    public bool JoinMuted { get; } = false; // Default: not muted
}
