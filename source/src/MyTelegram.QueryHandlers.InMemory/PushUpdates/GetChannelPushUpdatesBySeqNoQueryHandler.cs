//namespace MyTelegram.QueryHandlers.InMemory.PushUpdates;

//public class GetChannelPushUpdatesBySeqNoQueryHandler : IQueryHandler<GetChannelPushUpdatesBySeqNoQuery,
//    IReadOnlyCollection<IPushUpdatesReadModel>>
//{
//    private readonly IQueryOnlyReadModelStore<PushUpdatesReadModel> _store;

//    public GetChannelPushUpdatesBySeqNoQueryHandler(IQueryOnlyReadModelStore<PushUpdatesReadModel> store)
//    {
//        _store = store;
//    }

//    public async Task<IReadOnlyCollection<IPushUpdatesReadModel>> ExecuteQueryAsync(
//        GetChannelPushUpdatesBySeqNoQuery query,
//        CancellationToken cancellationToken)
//    {
//        var options = new FindOptions<PushUpdatesReadModel, PushUpdatesReadModel> { Limit = query.Limit, Skip = 0 };

//        var cursor = await _store.FindAsync(p => p.SeqNo > query.SeqNo && query.ChannelIdList.Contains(p.PeerId),
//            options,
//            cancellationToken);
//        return await cursor.ToListAsync(cancellationToken);
//    }
//}
