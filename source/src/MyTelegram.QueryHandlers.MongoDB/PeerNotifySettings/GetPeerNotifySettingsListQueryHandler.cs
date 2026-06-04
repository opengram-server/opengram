//namespace MyTelegram.QueryHandlers.MongoDB.PeerNotifySettings;

//public class GetPeerNotifySettingsListQueryHandler(IQueryOnlyReadModelStore<PeerNotifySettingsReadModel> store)
//    : IQueryHandler<GetPeerNotifySettingsListQuery,
//    IReadOnlyCollection<IPeerNotifySettingsReadModel>>
//{
//    public async Task<IReadOnlyCollection<IPeerNotifySettingsReadModel>> ExecuteQueryAsync(
//        GetPeerNotifySettingsListQuery query,
//        CancellationToken cancellationToken)
//    {
//        var peerIdList = query.PeerNotifySettingsIdList.Select(p => p.Value).ToList();
//        return await store.FindAsync(p => peerIdList.Contains(p.Id), cancellationToken: cancellationToken);
//    }
//}
