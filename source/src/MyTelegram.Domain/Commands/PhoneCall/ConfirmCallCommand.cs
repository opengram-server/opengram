using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Commands.PhoneCall;

public class ConfirmCallCommand(
    PhoneCallId aggregateId,
    RequestInfo requestInfo,
    byte[] gA,
    long keyFingerprint,
    PhoneCallProtocol protocol,
    List<PhoneConnectionInfo> connections,
    int startDate)
    : RequestCommand2<PhoneCallAggregate, PhoneCallId, IExecutionResult>(aggregateId, requestInfo)
{
    public byte[] GA { get; } = gA;
    public long KeyFingerprint { get; } = keyFingerprint;
    public PhoneCallProtocol Protocol { get; } = protocol;
    public List<PhoneConnectionInfo> Connections { get; } = connections;
    public int StartDate { get; } = startDate;
}
