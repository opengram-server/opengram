namespace MyTelegram.QueryHandlers.MongoDB.RpcResult;

public class
    GetRpcResultListQueryHandler(IQueryOnlyReadModelStore<RpcResultReadModel> store) : IQueryHandler<GetRpcResultListQuery, IReadOnlyCollection<RpcResultSimpleItem>>
{
    public async Task<IReadOnlyCollection<RpcResultSimpleItem>> ExecuteQueryAsync(GetRpcResultListQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.Date < query.ExpirationDate, p => new RpcResultSimpleItem(p.UserId, p.ReqMsgId), limit: query.Limit, cancellationToken: cancellationToken);
    }
}