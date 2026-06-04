namespace MyTelegram.QueryHandlers.MongoDB.Dialog;

public class GetDialogsQueryHandler(IQueryOnlyReadModelStore<DialogReadModel> store) : IQueryHandler<GetDialogsQuery, IReadOnlyCollection<IDialogReadModel>>
{
    public async Task<IReadOnlyCollection<IDialogReadModel>> ExecuteQueryAsync(GetDialogsQuery query,
        CancellationToken cancellationToken)
    {
        // Fix native aot mission metadata issues
        var needPinnedParameter = false;
        var needOffsetDate = false;
        var pinned = false;
        var offsetDate = DateTime.UtcNow;
        if (query.Pinned.HasValue)
        {
            needPinnedParameter = true;
            pinned = query.Pinned.Value;
        }

        if (query.OffsetDate.HasValue)
        {
            needOffsetDate = true;
            offsetDate = query.OffsetDate.Value;
        }

        Expression<Func<DialogReadModel, bool>> predicate = x => x.OwnerId == query.OwnerId && !x.IsDeleted;
        predicate = predicate
                .WhereIf(needOffsetDate, p => p.CreationTime > offsetDate)
                .WhereIf(needPinnedParameter, p => p.Pinned == pinned)
                .WhereIf(query.PeerIdList?.Count > 0, p => query.PeerIdList!.Contains(p.ToPeerId))
                .WhereIf(query.FolderId.HasValue && query.FolderId != 0, p => p.FolderId == query.FolderId)
            ;

        var sort = new SortOptions<DialogReadModel>(p => p.TopMessage, SortType.Descending);
        return await store.FindAsync(predicate, limit: query.Limit, sort: sort, cancellationToken: cancellationToken);
    }
}
