namespace MyTelegram.Domain.Commands.Dialog;

public class DeleteDialogFilterCommand(
    DialogFilterId aggregateId,
    RequestInfo requestInfo)
    : RequestCommand2<DialogFilterAggregate, DialogFilterId, IExecutionResult>(aggregateId, requestInfo);