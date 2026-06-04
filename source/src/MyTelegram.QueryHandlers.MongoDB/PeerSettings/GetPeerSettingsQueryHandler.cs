namespace MyTelegram.QueryHandlers.MongoDB.PeerSettings;

public class GetPeerSettingsQueryHandler(IQueryOnlyReadModelStore<PeerSettingsReadModel> store) : IQueryHandler<GetPeerSettingsQuery, IPeerSettingsReadModel?>
{
    public async Task<IPeerSettingsReadModel?> ExecuteQueryAsync(GetPeerSettingsQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.OwnerPeerId == query.SelfUserId && p.PeerId == query.PeerId, cancellationToken: cancellationToken);
    }
}