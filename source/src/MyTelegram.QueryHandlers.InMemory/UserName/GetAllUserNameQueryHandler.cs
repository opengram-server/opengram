namespace MyTelegram.QueryHandlers.InMemory.UserName;

public class GetAllUserNameQueryHandler(IQueryOnlyReadModelStore<UserNameReadModel> store) : IQueryHandler<GetAllUserNameQuery, IReadOnlyCollection<IUserNameReadModel>>
{
    public async Task<IReadOnlyCollection<IUserNameReadModel>> ExecuteQueryAsync(GetAllUserNameQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => true, p => p, query.Skip, query.Limit, cancellationToken: cancellationToken);
    }
}
