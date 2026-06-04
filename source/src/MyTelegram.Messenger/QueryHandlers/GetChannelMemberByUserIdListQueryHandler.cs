using EventFlow.Queries;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.Messenger.QueryHandlers;

public class GetChannelMemberByUserIdListQueryHandler(IQueryProcessor queryProcessor)
    : IQueryHandler<GetChannelMemberByUserIdListQuery, List<ChannelParticipantReadModel>>
{
    public async Task<List<ChannelParticipantReadModel>> ExecuteQueryAsync(
        GetChannelMemberByUserIdListQuery query,
        CancellationToken cancellationToken)
    {
        if (query.UserIds == null || query.UserIds.Count == 0)
        {
            return new List<ChannelParticipantReadModel>();
        }

        var members = await queryProcessor.ProcessAsync(
            new GetChannelMembersByChannelIdQuery(
                query.ChannelId, 
                query.UserIds,
                Offset: 0,
                Limit: query.UserIds.Count),
            cancellationToken);
        
        return members.OfType<ChannelParticipantReadModel>().ToList();
    }
}
