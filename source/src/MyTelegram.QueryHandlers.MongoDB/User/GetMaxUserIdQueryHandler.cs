namespace MyTelegram.QueryHandlers.MongoDB.User;

public class GetMaxUserIdQueryHandler(IQueryOnlyReadModelStore<UserReadModel> store) : IQueryHandler<GetMaxUserIdQuery, long>
{
    public async Task<long> ExecuteQueryAsync(GetMaxUserIdQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.UserId > 0, createResult: p => p.UserId, sort: new SortOptions<UserReadModel>(p => p.UserId, SortType.Descending), cancellationToken: cancellationToken);
    }
}