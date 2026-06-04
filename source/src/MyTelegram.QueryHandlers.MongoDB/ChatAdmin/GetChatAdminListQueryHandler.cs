namespace MyTelegram.QueryHandlers.MongoDB.ChatAdmin;

public class GetChatAdminListQueryHandler(IQueryOnlyReadModelStore<ChatAdminReadModel> store) : IQueryHandler<GetChatAdminListQuery,
    IReadOnlyCollection<IChatAdminReadModel>>
{
    public async Task<IReadOnlyCollection<IChatAdminReadModel>> ExecuteQueryAsync(GetChatAdminListQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => true, query.Skip, query.Limit, cancellationToken: cancellationToken);
    }
}