//namespace MyTelegram.QueryHandlers.InMemory.PushUpdates;

//public class
//    GetPushUpdatesByPtsQueryHandler : IQueryHandler<GetPushUpdatesByPtsQuery,
//        IReadOnlyCollection<IPushUpdatesReadModel>>
//{
//    private readonly IQueryOnlyReadModelStore<PushUpdatesReadModel> _store;

//    public GetPushUpdatesByPtsQueryHandler(IQueryOnlyReadModelStore<PushUpdatesReadModel> store)
//    {
//        _store = store;
//    }

//    public async Task<IReadOnlyCollection<IPushUpdatesReadModel>> ExecuteQueryAsync(GetPushUpdatesByPtsQuery query,
//        CancellationToken cancellationToken)
//    {
//        var options = new FindOptions<PushUpdatesReadModel, PushUpdatesReadModel> { Limit = query.Limit, Skip = 0 };

//        var cursor = await _store.FindAsync(p => p.PeerId == query.PeerId && p.Pts > query.Pts,
//            options,
//            cancellationToken);
//        return await cursor.ToListAsync(cancellationToken);
//    }
//}
