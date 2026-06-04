namespace MyTelegram.Domain.CommandHandlers.RpcResult;

public class CreateRpcResultCommandHandler : CommandHandler<RpcResultAggregate, RpcResultId, CreateRpcResultCommand>
{
    public override Task ExecuteAsync(RpcResultAggregate aggregate,
        CreateRpcResultCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Create(command.RequestInfo, command.RpcData);
        return Task.CompletedTask;
    }
}