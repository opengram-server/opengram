// ReSharper disable once CheckNamespace
namespace MyTelegram.Queries;

public class GetPtsByPermAuthKeyIdQueryHandler(IQueryOnlyReadModelStore<PtsForAuthKeyIdReadModel> store) : IQueryHandler<GetPtsByPermAuthKeyIdQuery, IPtsForAuthKeyIdReadModel?>
{
    public async Task<IPtsForAuthKeyIdReadModel?> ExecuteQueryAsync(GetPtsByPermAuthKeyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.PeerId == query.PeerId && p.PermAuthKeyId == query.PermAuthKeyId, cancellationToken: cancellationToken);
    }
}
