namespace MyTelegram.Domain.Commands.RpcResult;

public class DeleteRpcResultCommand(
    RpcResultId aggregateId)
    : Command<RpcResultAggregate, RpcResultId, IExecutionResult>(aggregateId)
{
}