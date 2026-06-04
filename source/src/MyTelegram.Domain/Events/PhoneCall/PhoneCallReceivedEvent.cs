using MyTelegram.Domain.Aggregates.PhoneCall;

namespace MyTelegram.Domain.Events.PhoneCall;

public class PhoneCallReceivedEvent(
    RequestInfo requestInfo,
    long callId,
    int receiveDate)
    : RequestAggregateEvent2<PhoneCallAggregate, PhoneCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public int ReceiveDate { get; } = receiveDate;
}
