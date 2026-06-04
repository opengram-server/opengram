using MyTelegram.Domain.Aggregates.PhoneCall;

namespace MyTelegram.Domain.Commands.PhoneCall;

public class SendSignalingDataCommand(
    PhoneCallId aggregateId,
    RequestInfo requestInfo,
    byte[] data)
    : RequestCommand2<PhoneCallAggregate, PhoneCallId, IExecutionResult>(aggregateId, requestInfo)
{
    public byte[] Data { get; } = data;
}
