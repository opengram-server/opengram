namespace MyTelegram.QueryHandlers.MongoDB.Dialog;

public class GetDialogsByFolderIdQueryHandler(IQueryOnlyReadModelStore<DialogReadModel> store)
    : IQueryHandler<GetDialogsByFolderIdQuery, IReadOnlyCollection<Peer>>
{
    public async Task<IReadOnlyCollection<Peer>> ExecuteQueryAsync(GetDialogsByFolderIdQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.OwnerId == query.OwnerUserId && p.FolderId == query.FolderId,
            p => new Peer(p.ToPeerType, p.ToPeerId), cancellationToken: cancellationToken);
    }
}