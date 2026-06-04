namespace MyTelegram.Domain.Commands.RpcResult;

public class CreateRpcResultCommand(
    RpcResultId aggregateId,
    RequestInfo requestInfo,
    byte[] rpcData)
    : RequestCommand2<RpcResultAggregate, RpcResultId, IExecutionResult>(aggregateId, requestInfo)
{
    public byte[] RpcData { get; } = rpcData;
}