namespace MyTelegram.QueryHandlers.InMemory.Pts;

public class GetMaxMessageIdByPeerIdQueryHandler(IQueryOnlyReadModelStore<PtsReadModel> store) : IQueryHandler<GetMaxMessageIdByPeerIdQuery, int>
{
    public async Task<int> ExecuteQueryAsync(GetMaxMessageIdByPeerIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.PeerId == query.PeerId, p => p.MaxMessageId, cancellationToken: cancellationToken);
    }
}