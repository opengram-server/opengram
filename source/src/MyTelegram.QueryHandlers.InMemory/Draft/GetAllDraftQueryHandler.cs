namespace MyTelegram.QueryHandlers.InMemory.Draft;

public class GetAllDraftQueryHandler(IQueryOnlyReadModelStore<DraftReadModel> store) : IQueryHandler<GetAllDraftQuery, IReadOnlyCollection<IDraftReadModel>>
{
    public async Task<IReadOnlyCollection<IDraftReadModel>> ExecuteQueryAsync(GetAllDraftQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.OwnerPeerId == query.OwnerPeerId, cancellationToken: cancellationToken);
    }
}
