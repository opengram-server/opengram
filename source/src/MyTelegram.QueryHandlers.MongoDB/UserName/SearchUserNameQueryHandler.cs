namespace MyTelegram.QueryHandlers.MongoDB.UserName;

public class SearchUserNameQueryHandler(IQueryOnlyReadModelStore<UserNameReadModel> store) : IQueryHandler<SearchUserNameQuery, IReadOnlyCollection<IUserNameReadModel>>
{
    public async Task<IReadOnlyCollection<IUserNameReadModel>> ExecuteQueryAsync(SearchUserNameQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.UserName.StartsWith(query.Keyword),
            limit: 50, cancellationToken: cancellationToken);
    }
}