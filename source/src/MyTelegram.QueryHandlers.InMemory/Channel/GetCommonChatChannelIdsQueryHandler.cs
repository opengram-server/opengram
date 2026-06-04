namespace MyTelegram.QueryHandlers.InMemory.Channel;

public class GetCommonChatChannelIdsQueryHandler
    (IQueryOnlyReadModelStore<ChannelMemberReadModel> store) : IQueryHandler<GetCommonChatChannelIdsQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetCommonChatChannelIdsQuery query,
        CancellationToken cancellationToken)
    {
        var joinedChannelIds = await store.FindAsync(p => p.UserId == query.SelfUserId && p.IsBroadcast == false && !p.Left, p => p.ChannelId, cancellationToken: cancellationToken);
        if (joinedChannelIds.Count > 0)
        {
            Expression<Func<ChannelMemberReadModel, bool>> predicate = p => joinedChannelIds.Contains(p.ChannelId) && p.UserId == query.TargetUserId && !p.Left;
            if (query.MaxId > 0)
            {
                predicate = predicate.And(p => p.ChannelId > query.MaxId);
            }

            var channelIds = await store.FindAsync(predicate, p => p.ChannelId,
                limit: query.Limit,
                cancellationToken: cancellationToken);

            return channelIds;
        }

        return [];
    }
}