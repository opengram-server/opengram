using MyTelegram.Domain.Aggregates.PhoneCall;

namespace MyTelegram.Domain.Events.PhoneCall;

public class PhoneCallDiscardedEvent(
    RequestInfo requestInfo,
    long callId,
    long adminId,
    long participantId,
    PhoneCallDiscardReason reason,
    int? duration,
    int date,
    bool isVideo)
    : RequestAggregateEvent2<PhoneCallAggregate, PhoneCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public long AdminId { get; } = adminId;
    public long ParticipantId { get; } = participantId;
    public PhoneCallDiscardReason Reason { get; } = reason;
    public int? Duration { get; } = duration;
    public int Date { get; } = date;
    public bool IsVideo { get; } = isVideo;
}
