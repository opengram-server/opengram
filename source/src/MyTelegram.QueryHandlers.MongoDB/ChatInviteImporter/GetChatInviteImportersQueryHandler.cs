//namespace MyTelegram.QueryHandlers.MongoDB.ChatInviteImporter;
//public class GetChatInviteImportersQueryHandler(IQueryOnlyReadModelStore<ChatInviteImporterReadModel> store)
//    : IQueryHandler<GetChatInviteImportersQuery, IReadOnlyCollection<IChatInviteImporterReadModel>>
//{
//    public async Task<IReadOnlyCollection<IChatInviteImporterReadModel>> ExecuteQueryAsync(GetChatInviteImportersQuery query, CancellationToken cancellationToken)
//    {
//        Expression<Func<ChatInviteImporterReadModel, bool>> predicate = x => x.PeerId == query.ChannelId &&
//                                                                             //x.ChatInviteRequestState == query.ChatInviteRequestState &&
//                                                                             x.Date > query.OffsetDate;
//        predicate = predicate.WhereIf(query.ChatInviteRequestState.HasValue,
//            x => x.ChatInviteRequestState == query.ChatInviteRequestState);

//        return await store.FindAsync(predicate, limit: query.Limit, cancellationToken: cancellationToken);
//    }
//}