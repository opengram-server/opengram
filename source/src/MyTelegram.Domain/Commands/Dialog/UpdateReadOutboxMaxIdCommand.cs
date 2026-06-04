namespace MyTelegram.Domain.Commands.Dialog;

public class UpdateReadOutboxMaxIdCommand
    (DialogId aggregateId, RequestInfo requestInfo, int maxId) :
        RequestCommand2<DialogAggregate, DialogId, IExecutionResult>(aggregateId, requestInfo)
{
    public int MaxId { get; } = maxId;
}