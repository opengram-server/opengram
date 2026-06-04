namespace MyTelegram.QueryHandlers.InMemory.Channel;

public class
    GetChatInviteImportersQueryHandler(IQueryOnlyReadModelStore<JoinChannelRequestReadModel> store) : IQueryHandler<GetChatInviteImportersQuery, IReadOnlyCollection<IJoinChannelRequestReadModel>>
{
    public async Task<IReadOnlyCollection<IJoinChannelRequestReadModel>> ExecuteQueryAsync(GetChatInviteImportersQuery query, CancellationToken cancellationToken)
    {
        Expression<Func<JoinChannelRequestReadModel, bool>> predicate = x => x.ChannelId == query.ChannelId;
        predicate = predicate.WhereIf(query.ChatInviteRequestState == ChatInviteRequestState.WaitingForApproval,
                    p => !p.IsJoinRequestProcessed)
                .WhereIf(query.InviteId > 0, p => p.InviteId == query.InviteId)
                .WhereIf(query.OffsetDate.HasValue, p => p.Date > query.OffsetDate)
            ;

        return await store.FindAsync(predicate, limit: query.Limit, cancellationToken: cancellationToken);
    }
}