using MyTelegram.Domain.Shared.Business;
using MyTelegram.Queries.Business;

namespace MyTelegram.QueryHandlers.MongoDB.Business;

public class GetConnectedBotsQueryHandler(IQueryOnlyReadModelStore<UserReadModel> store) 
    : IQueryHandler<GetConnectedBotsQuery, List<ConnectedBot>>
{
    public async Task<List<ConnectedBot>> ExecuteQueryAsync(GetConnectedBotsQuery query, CancellationToken cancellationToken)
    {
        var user = await store.FirstOrDefaultAsync(
            p => p.UserId == query.UserId, 
            createResult: p => p, 
            cancellationToken: cancellationToken);
        
        // TODO: Implement ConnectedBots storage in UserReadModel
        // For now, return empty list
        return new List<ConnectedBot>();
    }
}
