namespace MyTelegram.QueryHandlers.InMemory.UserName;

public class GetUserNameByIdQueryHandler(IQueryOnlyReadModelStore<UserNameReadModel> store) : IQueryHandler<GetUserNameByIdQuery, IUserNameReadModel?>
{
    public async Task<IUserNameReadModel?> ExecuteQueryAsync(GetUserNameByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.UserName == query.UserName, cancellationToken);
    }
}
