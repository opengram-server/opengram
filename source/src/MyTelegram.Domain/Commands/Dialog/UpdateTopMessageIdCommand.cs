namespace MyTelegram.Domain.Commands.Dialog;

public class UpdateTopMessageIdCommand(DialogId aggregateId, int newTopMessageId)
    : Command<DialogAggregate, DialogId, IExecutionResult>(aggregateId)
{
    public int NewTopMessageId { get; } = newTopMessageId;
}