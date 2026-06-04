using EventFlow.Queries;
using EventFlow.ReadStores;
using MyTelegram.Queries.Privacy;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.Privacy;

public class GetPrivacyByUserIdQueryHandler(IQueryOnlyReadModelStore<PrivacyReadModel> store)
    : IQueryHandler<GetPrivacyByUserIdQuery, IReadOnlyCollection<IPrivacyReadModel>>
{
    public async Task<IReadOnlyCollection<IPrivacyReadModel>> ExecuteQueryAsync(
        GetPrivacyByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        // Filter by UserId and optionally by PrivacyType (converted from Key)
        if (query.Key != null)
        {
            var privacyType = query.Key.ToPrivacyType();
            return await store.FindAsync(
                p => p.UserId == query.UserId && p.PrivacyType == privacyType,
                cancellationToken: cancellationToken);
        }
        
        // Return all privacy settings for user if no key specified
        return await store.FindAsync(
            p => p.UserId == query.UserId,
            cancellationToken: cancellationToken);
    }
}
