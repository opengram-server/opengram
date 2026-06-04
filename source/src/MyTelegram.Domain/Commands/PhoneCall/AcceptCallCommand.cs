using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Commands.PhoneCall;

public class AcceptCallCommand(
    PhoneCallId aggregateId,
    RequestInfo requestInfo,
    byte[] gB,
    PhoneCallProtocol protocol)
    : RequestCommand2<PhoneCallAggregate, PhoneCallId, IExecutionResult>(aggregateId, requestInfo)
{
    public byte[] GB { get; } = gB;
    public PhoneCallProtocol Protocol { get; } = protocol;
}
