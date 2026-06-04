// ReSharper disable once CheckNamespace

namespace MyTelegram.Queries;
//.MongoDB.PeerNotifySettings

public class
    GetPeerNotifySettingsByIdQueryHandler(IQueryOnlyReadModelStore<PeerNotifySettingsReadModel> store)
    : IQueryHandler<GetPeerNotifySettingsByIdQuery,
        IPeerNotifySettingsReadModel>
{
    public async Task<IPeerNotifySettingsReadModel> ExecuteQueryAsync(GetPeerNotifySettingsByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken) ?? new PeerNotifySettingsReadModel();
    }
}
