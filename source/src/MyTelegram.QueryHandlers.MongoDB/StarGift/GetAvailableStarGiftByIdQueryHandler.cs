using MongoDB.Driver;
using MyTelegram.Queries.StarGift;
using MyTelegram.ReadModel;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.StarGift;

public class GetAvailableStarGiftByIdQueryHandler(IMongoDatabase database)
    : IQueryHandler<GetAvailableStarGiftByIdQuery, IAvailableStarGiftReadModel?>
{
    public async Task<IAvailableStarGiftReadModel?> ExecuteQueryAsync(
        GetAvailableStarGiftByIdQuery query,
        CancellationToken cancellationToken)
    {
        var collection = database.GetCollection<AvailableStarGiftReadModel>("AvailableStarGiftReadModel");
        
        var filter = Builders<AvailableStarGiftReadModel>.Filter.Eq(x => x.GiftId, query.GiftId);
        
        var gift = await collection.Find(filter)
            .FirstOrDefaultAsync(cancellationToken);
        
        return gift;
    }
}
