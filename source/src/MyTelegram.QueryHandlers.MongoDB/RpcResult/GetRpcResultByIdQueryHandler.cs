namespace MyTelegram.QueryHandlers.MongoDB.RpcResult;

public class GetRpcResultByIdQueryHandler(IQueryOnlyReadModelStore<RpcResultReadModel> store) : IQueryHandler<GetRpcResultByIdQuery, IRpcResultReadModel?>
{
    public async Task<IRpcResultReadModel?> ExecuteQueryAsync(GetRpcResultByIdQuery query,
        CancellationToken cancellationToken)
    {
        //var item = await _store.GetAsync(query.Id, cancellationToken);
        //return item.ReadModel;
        return await store.FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken);
    }
}