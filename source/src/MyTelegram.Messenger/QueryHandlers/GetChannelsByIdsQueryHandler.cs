using EventFlow.Queries;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.Messenger.QueryHandlers;

public class GetChannelsByIdsQueryHandler(IQueryProcessor queryProcessor)
    : IQueryHandler<GetChannelsByIdsQuery, List<ChannelReadModel>>
{
    public async Task<List<ChannelReadModel>> ExecuteQueryAsync(
        GetChannelsByIdsQuery query,
        CancellationToken cancellationToken)
    {
        if (query.ChannelIds == null || query.ChannelIds.Count == 0)
        {
            return new List<ChannelReadModel>();
        }

        var channels = await queryProcessor.ProcessAsync(
            new GetChannelByChannelIdListQuery(query.ChannelIds),
            cancellationToken);
        
        return channels.OfType<ChannelReadModel>().ToList();
    }
}
