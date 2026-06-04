namespace MyTelegram.Domain.Commands.Dialog;

public class
    ClearChannelHistoryCommand(DialogId aggregateId,
        RequestInfo requestInfo, int availableMinId)
    : RequestCommand2<DialogAggregate, DialogId, IExecutionResult>(aggregateId, requestInfo) //, IHasCorrelationId
{
    public int AvailableMinId { get; } = availableMinId;

    //public Guid CorrelationId { get; }
}