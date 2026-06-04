using MyTelegram.Domain.Aggregates.PhoneCall;

namespace MyTelegram.Domain.Commands.PhoneCall;

public class ReceivedCallCommand(
    PhoneCallId aggregateId,
    RequestInfo requestInfo,
    int receiveDate)
    : RequestCommand2<PhoneCallAggregate, PhoneCallId, IExecutionResult>(aggregateId, requestInfo)
{
    public int ReceiveDate { get; } = receiveDate;
}
