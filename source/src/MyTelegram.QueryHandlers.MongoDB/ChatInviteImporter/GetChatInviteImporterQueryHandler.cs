namespace MyTelegram.QueryHandlers.MongoDB.ChatInviteImporter;

public class GetChatInviteImporterQueryHandler(IQueryOnlyReadModelStore<ChatInviteImporterReadModel> store)
    : IQueryHandler<GetChatInviteImporterQuery,
    IChatInviteImporterReadModel?>
{
    public async Task<IChatInviteImporterReadModel?> ExecuteQueryAsync(GetChatInviteImporterQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.PeerId == query.PeerId && p.UserId == query.UserId, cancellationToken: cancellationToken);
    }
}