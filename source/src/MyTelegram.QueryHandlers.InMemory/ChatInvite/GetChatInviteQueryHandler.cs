namespace MyTelegram.QueryHandlers.InMemory.ChatInvite;

public class GetChatInviteQueryHandler(IQueryOnlyReadModelStore<ChatInviteReadModel> store) : IQueryHandler<GetChatInviteQuery, IChatInviteReadModel?>
{
    public async Task<IChatInviteReadModel?> ExecuteQueryAsync(GetChatInviteQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.PeerId == query.PeerId && p.Link == query.Link, cancellationToken: cancellationToken);
    }
}