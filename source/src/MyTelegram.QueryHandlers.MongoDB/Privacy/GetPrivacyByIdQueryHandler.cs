using MyTelegram.Queries.Privacy;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.Privacy;

public class GetPrivacyByIdQueryHandler(IQueryOnlyReadModelStore<PrivacyReadModel> store)
    : IQueryHandler<GetPrivacyByIdQuery, IPrivacyReadModel?>
{
    public async Task<IPrivacyReadModel?> ExecuteQueryAsync(GetPrivacyByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.Id == query.PrivacyId, cancellationToken);
    }
}
