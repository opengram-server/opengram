namespace MyTelegram.QueryHandlers.InMemory.ChatInvite;

public class
    GetRevokedChatInvitesQueryHandler(IQueryOnlyReadModelStore<ChatInviteReadModel> store) : IQueryHandler<GetRevokedChatInvitesQuery,
        IReadOnlyCollection<IChatInviteReadModel>>
{
    public async Task<IReadOnlyCollection<IChatInviteReadModel>> ExecuteQueryAsync(GetRevokedChatInvitesQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.PeerId == query.PeerId && p.Revoked && p.AdminId == query.AdminId, cancellationToken: cancellationToken);
    }
}