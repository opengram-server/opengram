//namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

//public class GetMaxMessageIdByUserIdQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetMaxMessageIdByUserIdQuery, int>
//{
//    public async Task<int> ExecuteQueryAsync(GetMaxMessageIdByUserIdQuery query, CancellationToken cancellationToken)
//    {
//        return await store.FirstOrDefaultAsync(p => p.OwnerPeerId == query.UserId && p.ToPeerType != PeerType.Channel,
//            p => p.MessageId,
//            sort: new SortOptions<MessageReadModel>(p => p.MessageId, SortType.Descending)
//            , cancellationToken
//        );
//    }
//}