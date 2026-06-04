namespace MyTelegram.Domain.Commands.Dialog;

public class ClearDraftCommand(DialogId aggregateId, RequestInfo requestInfo)
    : RequestCommand2<DialogAggregate, DialogId, IExecutionResult>(aggregateId, requestInfo);