namespace MyTelegram.QueryHandlers.InMemory.ChatInviteImporter;

public class GetChatInviteImporterListForApprovalQueryHandler(
        IQueryOnlyReadModelStore<ChatInviteImporterReadModel> store)
    : IQueryHandler<GetChatInviteImporterListForApprovalQuery,
    IReadOnlyCollection<IChatInviteImporterReadModel>>
{
    public async Task<IReadOnlyCollection<IChatInviteImporterReadModel>> ExecuteQueryAsync(GetChatInviteImporterListForApprovalQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.PeerId == query.PeerId && p.InviteId == query.InviteId && p.ChatInviteRequestState == ChatInviteRequestState.WaitingForApproval, cancellationToken: cancellationToken);
    }
}