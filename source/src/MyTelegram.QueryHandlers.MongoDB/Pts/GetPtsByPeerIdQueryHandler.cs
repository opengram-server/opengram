namespace MyTelegram.QueryHandlers.MongoDB.Pts;

public class GetPtsByPeerIdQueryHandler(IQueryOnlyReadModelStore<PtsReadModel> store) : IQueryHandler<GetPtsByPeerIdQuery, IPtsReadModel?>
{
    public async Task<IPtsReadModel?> ExecuteQueryAsync(GetPtsByPeerIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.PeerId == query.PeerId, cancellationToken);
    }
}