namespace MyTelegram.QueryHandlers.MongoDB.ChatInvite;

public class GetChatInviteByLinkQueryHandler(IQueryOnlyReadModelStore<ChatInviteReadModel> store) : IQueryHandler<GetChatInviteByLinkQuery, IChatInviteReadModel?>
{
    public async Task<IChatInviteReadModel?> ExecuteQueryAsync(GetChatInviteByLinkQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.Link == query.Link, cancellationToken: cancellationToken);
    }
}