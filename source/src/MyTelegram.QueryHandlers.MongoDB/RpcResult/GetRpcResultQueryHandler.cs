namespace MyTelegram.QueryHandlers.MongoDB.RpcResult;

public class GetRpcResultQueryHandler(IQueryOnlyReadModelStore<RpcResultReadModel> store) : IQueryHandler<GetRpcResultQuery, IRpcResultReadModel?>
{
    public async Task<IRpcResultReadModel?> ExecuteQueryAsync(GetRpcResultQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.UserId == query.UserId && p.ReqMsgId == query.ReqMsgId, cancellationToken: cancellationToken);
    }
}