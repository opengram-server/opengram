namespace MyTelegram.QueryHandlers.InMemory.Channel;

public class GetCommonChatCountQueryHandler
    (IQueryOnlyReadModelStore<ChannelMemberReadModel> store) : IQueryHandler<GetCommonChatCountQuery, int>
{
    public async Task<int> ExecuteQueryAsync(GetCommonChatCountQuery query,
        CancellationToken cancellationToken)
    {
        var joinedChannelIds = await store.FindAsync(p => p.UserId == query.SelfUserId && p.IsBroadcast == false, p => p.ChannelId, cancellationToken: cancellationToken);
        if (joinedChannelIds.Count > 0)
        {
            return (int)await store.CountAsync(p =>
                joinedChannelIds.Contains(p.ChannelId) && p.UserId == query.TargetUserId, cancellationToken);
        }

        return 0;
    }
}