namespace MyTelegram.Domain.Commands.Dialog;

public class SetPinnedOrderCommand(
    DialogId aggregateId,
    int order) : Command<DialogAggregate, DialogId, IExecutionResult>(aggregateId)
{
    public int Order { get; } = order;
}
