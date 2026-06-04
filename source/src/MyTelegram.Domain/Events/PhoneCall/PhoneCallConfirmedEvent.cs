using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Events.PhoneCall;

public class PhoneCallConfirmedEvent(
    RequestInfo requestInfo,
    long callId,
    long accessHash,
    long adminId,
    long participantId,
    bool isVideo,
    byte[] gA,
    long keyFingerprint,
    PhoneCallProtocol protocol,
    List<PhoneConnectionInfo> connections,
    int startDate)
    : RequestAggregateEvent2<PhoneCallAggregate, PhoneCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public long AccessHash { get; } = accessHash;
    public long AdminId { get; } = adminId;
    public long ParticipantId { get; } = participantId;
    public bool IsVideo { get; } = isVideo;
    public byte[] GA { get; } = gA;
    public long KeyFingerprint { get; } = keyFingerprint;
    public PhoneCallProtocol Protocol { get; } = protocol;
    public List<PhoneConnectionInfo> Connections { get; } = connections;
    public int StartDate { get; } = startDate;
}
