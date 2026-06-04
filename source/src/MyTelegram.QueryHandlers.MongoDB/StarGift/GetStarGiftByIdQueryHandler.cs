using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.StarGift;

public class GetStarGiftByIdQueryHandler(IQueryOnlyReadModelStore<StarGiftReadModel> store) 
    : IQueryHandler<GetStarGiftByIdQuery, IStarGiftReadModel?>
{
    public async Task<IStarGiftReadModel?> ExecuteQueryAsync(GetStarGiftByIdQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(x => x.Id == query.GiftInstanceId, cancellationToken);
    }
}
