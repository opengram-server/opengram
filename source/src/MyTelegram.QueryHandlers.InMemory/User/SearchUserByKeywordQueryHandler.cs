namespace MyTelegram.QueryHandlers.InMemory.User;

public class SearchUserByKeywordQueryHandler(IQueryOnlyReadModelStore<UserReadModel> store) :
    IQueryHandler<SearchUserByKeywordQuery, IReadOnlyCollection<IUserReadModel>>
{
    public async Task<IReadOnlyCollection<IUserReadModel>> ExecuteQueryAsync(SearchUserByKeywordQuery query,
        CancellationToken cancellationToken)
    {
        var q = query.Keyword;
        if (!string.IsNullOrEmpty(q) && q.StartsWith('@'))
        {
            q = query.Keyword[1..];
        }

        Expression<Func<UserReadModel, bool>> predicate = x => true;
        predicate = predicate.WhereIf(!string.IsNullOrEmpty(q),
            p => (p.UserName != null && p.UserName.StartsWith(q)) ||
                 p.FirstName.Contains(q) ||
                 (p.LastName != null && p.LastName.StartsWith(q))
                 );

        return await store.FindAsync(predicate, 0, 50, new SortOptions<UserReadModel>(p => p.FirstName, SortType.Ascending), cancellationToken);
    }
}