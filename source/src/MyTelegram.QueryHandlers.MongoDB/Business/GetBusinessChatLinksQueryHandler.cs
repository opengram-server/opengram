using MyTelegram.Domain.Shared.Business;
using MyTelegram.Queries.Business;

namespace MyTelegram.QueryHandlers.MongoDB.Business;

public class GetBusinessChatLinksQueryHandler(IQueryOnlyReadModelStore<UserReadModel> store) 
    : IQueryHandler<GetBusinessChatLinksQuery, List<BusinessChatLink>>
{
    public async Task<List<BusinessChatLink>> ExecuteQueryAsync(GetBusinessChatLinksQuery query, CancellationToken cancellationToken)
    {
        var links = await store.FirstOrDefaultAsync(
            p => p.UserId == query.UserId, 
            createResult: p => p.BusinessChatLinks ?? new List<BusinessChatLink>(), 
            cancellationToken: cancellationToken);
        
        return links?.ToList() ?? new List<BusinessChatLink>();
    }
}
