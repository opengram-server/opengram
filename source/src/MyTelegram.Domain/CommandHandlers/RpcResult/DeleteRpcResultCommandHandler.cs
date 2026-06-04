namespace MyTelegram.Domain.CommandHandlers.RpcResult;

public class DeleteRpcResultCommandHandler : CommandHandler<RpcResultAggregate, RpcResultId, DeleteRpcResultCommand>
{
    public override Task ExecuteAsync(RpcResultAggregate aggregate,
        DeleteRpcResultCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Delete();
        return Task.CompletedTask;
    }
}