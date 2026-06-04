using MyTelegram.Queries.Privacy;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.Privacy;

public class GetUserPrivacyListQueryHandler(IQueryOnlyReadModelStore<PrivacyReadModel> store)
    : IQueryHandler<GetUserPrivacyListQuery, IReadOnlyCollection<IPrivacyReadModel>>
{
    public async Task<IReadOnlyCollection<IPrivacyReadModel>> ExecuteQueryAsync(GetUserPrivacyListQuery query,
        CancellationToken cancellationToken)
    {
        var results = await store.FindAsync(p => p.UserId == query.UserId);
        return results.Cast<IPrivacyReadModel>().ToList();
    }
}
