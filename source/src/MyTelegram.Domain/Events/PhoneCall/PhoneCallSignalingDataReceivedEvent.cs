using MyTelegram.Domain.Aggregates.PhoneCall;

namespace MyTelegram.Domain.Events.PhoneCall;

public class PhoneCallSignalingDataReceivedEvent(
    RequestInfo requestInfo,
    long callId,
    byte[] data)
    : RequestAggregateEvent2<PhoneCallAggregate, PhoneCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public byte[] Data { get; } = data;
}
