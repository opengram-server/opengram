namespace MyTelegram.Domain.Commands.Dialog;

public class SetDialogHistoryTTLCommand(
    DialogId aggregateId,
    RequestInfo requestInfo,
    Peer peer,
    int? ttlPeriod)
    : RequestCommand2<DialogAggregate, DialogId, IExecutionResult>(aggregateId, requestInfo)
{
    public Peer Peer { get; } = peer;
    public int? TtlPeriod { get; } = ttlPeriod;
}
