//namespace MyTelegram.QueryHandlers.InMemory.PushUpdates;

//public class GetPushUpdatesQueryHandler : IQueryHandler<GetPushUpdatesQuery, IReadOnlyCollection<IPushUpdatesReadModel>>
//{
//    private readonly IQueryOnlyReadModelStore<PushUpdatesReadModel> _store;

//    public GetPushUpdatesQueryHandler(IQueryOnlyReadModelStore<PushUpdatesReadModel> store)
//    {
//        _store = store;
//    }

//    public async Task<IReadOnlyCollection<IPushUpdatesReadModel>> ExecuteQueryAsync(GetPushUpdatesQuery query,
//        CancellationToken cancellationToken)
//    {
//        Expression<Func<PushUpdatesReadModel, bool>> predicate = p => p.PeerId == query.PeerId && p.Pts > query.MinPts;
//        if (query.Date > 0)
//        {
//            predicate = predicate.And(p => p.Date > query.Date);
//        }

//        var options = new FindOptions<PushUpdatesReadModel, PushUpdatesReadModel> { Limit = query.Limit, Skip = 0 };

//        var cursor = await _store.FindAsync(predicate,
//            options,
//            cancellationToken);
//        return await cursor.ToListAsync(cancellationToken);
//    }
//}
