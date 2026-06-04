namespace MyTelegram.QueryHandlers.MongoDB.User;

public class SearchUserByKeywordQueryHandler(IQueryOnlyReadModelStore<UserReadModel> store) :
    IQueryHandler<SearchUserByKeywordQuery, IReadOnlyCollection<IUserReadModel>>
{
    public async Task<IReadOnlyCollection<IUserReadModel>> ExecuteQueryAsync(SearchUserByKeywordQuery query,
        CancellationToken cancellationToken)
    {
        var q = query.Keyword;
        
        // Проверяем и очищаем поисковую строку перед использованием
        if (!string.IsNullOrEmpty(q))
        {
            // Ограничиваем длину, чтобы защититься от ReDoS
            if (q.Length > 256)
            {
                q = q.Substring(0, 256);
            }

            // Убираем спецсимволы регулярных выражений
            q = System.Text.RegularExpressions.Regex.Replace(q, @"[^\w\s@]", "");
            
            if (q.StartsWith('@'))
            {
                q = q[1..];
            }
        }

        Expression<Func<UserReadModel, bool>> predicate = x => true;
        predicate = predicate.WhereIf(!string.IsNullOrEmpty(q),
            p => (p.UserName != null && p.UserName.Contains(q)) ||
                 p.FirstName.Contains(q) ||
                 (p.LastName != null && p.LastName.Contains(q))
                 );

        return await store.FindAsync(predicate, 0, 50, new SortOptions<UserReadModel>(p => p.FirstName, SortType.Ascending), cancellationToken);
    }
}