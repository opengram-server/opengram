using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Events.PhoneCall;

public class PhoneCallRequestedEvent(
    RequestInfo requestInfo,
    long callId,
    long accessHash,
    long adminId,
    long participantId,
    bool isVideo,
    byte[] gAHash,
    PhoneCallProtocol protocol,
    int date)
    : RequestAggregateEvent2<PhoneCallAggregate, PhoneCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public long AccessHash { get; } = accessHash;
    public long AdminId { get; } = adminId;
    public long ParticipantId { get; } = participantId;
    public bool IsVideo { get; } = isVideo;
    public byte[] GAHash { get; } = gAHash;
    public PhoneCallProtocol Protocol { get; } = protocol;
    public int Date { get; } = date;
}
