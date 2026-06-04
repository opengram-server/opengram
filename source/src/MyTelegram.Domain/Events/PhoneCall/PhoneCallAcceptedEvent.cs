using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Events.PhoneCall;

public class PhoneCallAcceptedEvent(
    RequestInfo requestInfo,
    long callId,
    long accessHash,
    long adminId,
    long participantId,
    byte[] gB,
    PhoneCallProtocol protocol,
    bool isVideo)
    : RequestAggregateEvent2<PhoneCallAggregate, PhoneCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public long AccessHash { get; } = accessHash;
    public long AdminId { get; } = adminId;
    public long ParticipantId { get; } = participantId;
    public byte[] GB { get; } = gB;
    public PhoneCallProtocol Protocol { get; } = protocol;
    public bool IsVideo { get; } = isVideo;
}
