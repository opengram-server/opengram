using System.Linq.Expressions;
using EventFlow.Queries;
using EventFlow.ReadStores;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class SearchChannelByKeywordQueryHandler(IQueryOnlyReadModelStore<ChannelReadModel> store) :
    IQueryHandler<SearchChannelByKeywordQuery, IReadOnlyCollection<IChannelReadModel>>
{
    public async Task<IReadOnlyCollection<IChannelReadModel>> ExecuteQueryAsync(SearchChannelByKeywordQuery query,
        CancellationToken cancellationToken)
    {
        var q = query.Keyword;
        
        // Проверяем и очищаем поисковую строку перед использованием
        if (!string.IsNullOrEmpty(q))
        {
            // Ограничиваем длину, чтобы исключить ReDoS
            if (q.Length > 256)
            {
                q = q.Substring(0, 256);
            }

            // Убираем специальные символы регулярных выражений
            q = System.Text.RegularExpressions.Regex.Replace(q, @"[^\w\s@]", "");
            
            if (q.StartsWith('@'))
            {
                q = q[1..];
            }
        }

        Expression<Func<ChannelReadModel, bool>> predicate = x => true;
        predicate = predicate.WhereIf(!string.IsNullOrEmpty(q),
            p => (p.UserName != null && p.UserName.StartsWith(q)) ||
                 p.Title.Contains(q)
                 );

        return await store.FindAsync(predicate, 0, query.Limit, new SortOptions<ChannelReadModel>(p => p.Title, SortType.Ascending), cancellationToken);
    }
}
