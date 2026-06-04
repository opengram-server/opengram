namespace MyTelegram.QueryHandlers.MongoDB.ChatInvite;

public class
    GetChatInvitesQueryHandler(IQueryOnlyReadModelStore<ChatInviteReadModel> store) : IQueryHandler<GetChatInvitesQuery, IReadOnlyCollection<IChatInviteReadModel>>
{
    public async Task<IReadOnlyCollection<IChatInviteReadModel>> ExecuteQueryAsync(GetChatInvitesQuery query,
        CancellationToken cancellationToken)
    {
        var date = query.OffsetDate ?? 0;

        return await store.FindAsync(p =>
            p.Revoked == query.Revoked &&
            p.PeerId == query.PeerId &&
            p.AdminId == query.AdminId &&
            p.Date > date, limit: query.Limit, cancellationToken: cancellationToken);
    }
}