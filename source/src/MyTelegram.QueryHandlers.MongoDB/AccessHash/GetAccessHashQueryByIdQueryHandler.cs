namespace MyTelegram.QueryHandlers.MongoDB.AccessHash;

internal sealed class GetAccessHashQueryByIdQueryHandler(IQueryOnlyReadModelStore<AccessHashReadModel> store)
    : IQueryHandler<GetAccessHashQueryByIdQuery, IAccessHashReadModel?>
{
    public async Task<IAccessHashReadModel?> ExecuteQueryAsync(GetAccessHashQueryByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.AccessId == query.Id, cancellationToken);
    }
}