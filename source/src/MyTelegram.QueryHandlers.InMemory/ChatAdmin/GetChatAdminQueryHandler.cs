namespace MyTelegram.QueryHandlers.InMemory.ChatAdmin;

public class GetChatAdminQueryHandler(IQueryOnlyReadModelStore<ChatAdminReadModel> store) : IQueryHandler<GetChatAdminQuery, IChatAdminReadModel?>
{
    public async Task<IChatAdminReadModel?> ExecuteQueryAsync(GetChatAdminQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.PeerId == query.PeerId && p.UserId == query.AdminId, cancellationToken: cancellationToken);
    }
}