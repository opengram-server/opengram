using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MyTelegram.Queries.StarGift;
using MyTelegram.ReadModel;

namespace MyTelegram.QueryHandlers.MongoDB.StarGift;

public class GetResaleStarGiftByCollectibleIdQueryHandler : IQueryHandler<GetResaleStarGiftByCollectibleIdQuery, IStarGiftReadModel?>
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<GetResaleStarGiftByCollectibleIdQueryHandler> _logger;

    public GetResaleStarGiftByCollectibleIdQueryHandler(
        IMongoDatabase mongoDatabase,
        ILogger<GetResaleStarGiftByCollectibleIdQueryHandler> logger)
    {
        _mongoDatabase = mongoDatabase;
        _logger = logger;
    }

    public async Task<IStarGiftReadModel?> ExecuteQueryAsync(
        GetResaleStarGiftByCollectibleIdQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Looking for resale gift with CollectibleId={CollectibleId}", query.CollectibleId);

        var readModelCollection = _mongoDatabase.GetCollection<MyTelegram.ReadModel.Impl.StarGiftReadModel>("eventflow-stargiftreadmodel");
        
        var readModel = await readModelCollection
            .Find(x => x.CollectibleId == query.CollectibleId && x.ForResale == true && x.Upgraded == true)
            .FirstOrDefaultAsync(cancellationToken);

        if (readModel == null)
        {
            _logger.LogWarning("Resale gift not found: CollectibleId={CollectibleId}", query.CollectibleId);
            return null;
        }

        _logger.LogInformation("Found resale gift: AggregateId={AggregateId}, GiftId={GiftId}, Owner={Owner}, Price={Price}",
            readModel.AggregateId, readModel.GiftId, readModel.ToUserId, readModel.ResaleStars);

        return readModel;
    }
}
