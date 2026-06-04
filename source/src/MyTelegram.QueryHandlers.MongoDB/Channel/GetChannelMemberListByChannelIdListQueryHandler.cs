namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class GetChannelMemberListByChannelIdListQueryHandler(IQueryOnlyReadModelStore<ChannelMemberReadModel> store) :
    IQueryHandler<
    GetChannelMemberListByChannelIdListQuery, IReadOnlyCollection<IChannelMemberReadModel>>
{
    public async Task<IReadOnlyCollection<IChannelMemberReadModel>> ExecuteQueryAsync(
        GetChannelMemberListByChannelIdListQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.UserId == query.MemberUserId && query.ChannelIdList.Contains(p.ChannelId), cancellationToken: cancellationToken);
    }
}
