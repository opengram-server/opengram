
namespace MyTelegram.QueryHandlers.InMemory.Channel;

public class
    GetMegaGroupByUserIdQueryHandler(IQueryOnlyReadModelStore<ChannelReadModel> store) : IQueryHandler<GetMegaGroupByUserIdQuery, IReadOnlyCollection<IChannelReadModel>>
{
    public async Task<IReadOnlyCollection<IChannelReadModel>> ExecuteQueryAsync(GetMegaGroupByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.MegaGroup && p.CreatorId == query.UserId, cancellationToken: cancellationToken);
    }
}