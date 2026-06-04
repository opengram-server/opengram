using MyTelegram.Domain.Aggregates.PhoneCall;

namespace MyTelegram.Domain.Commands.PhoneCall;

public class DiscardCallCommand(
    PhoneCallId aggregateId,
    RequestInfo requestInfo,
    PhoneCallDiscardReason reason,
    int? duration,
    int date,
    bool isVideo)
    : RequestCommand2<PhoneCallAggregate, PhoneCallId, IExecutionResult>(aggregateId, requestInfo)
{
    public PhoneCallDiscardReason Reason { get; } = reason;
    public int? Duration { get; } = duration;
    public int Date { get; } = date;
    public bool IsVideo { get; } = isVideo;
}
