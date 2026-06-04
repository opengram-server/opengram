namespace MyTelegram.QueryHandlers.InMemory.User;

public class GetUserByIdQueryHandler(IQueryOnlyReadModelStore<UserReadModel> store) : IQueryHandler<GetUserByIdQuery, IUserReadModel?>
{
    public async Task<IUserReadModel?> ExecuteQueryAsync(GetUserByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.UserId == query.UserId, cancellationToken);
    }
}