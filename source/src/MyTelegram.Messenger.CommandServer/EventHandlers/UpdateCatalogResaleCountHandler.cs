using MongoDB.Bson;
using MongoDB.Driver;

namespace MyTelegram.Messenger.CommandServer.EventHandlers;

public class UpdateCatalogResaleCountHandler :
    IEventHandler<StarGiftListedForResaleIntegrationEvent>,
    IEventHandler<StarGiftRemovedFromResaleIntegrationEvent>
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<UpdateCatalogResaleCountHandler> _logger;

    public UpdateCatalogResaleCountHandler(
        IMongoDatabase mongoDatabase,
        ILogger<UpdateCatalogResaleCountHandler> logger)
    {
        _mongoDatabase = mongoDatabase;
        _logger = logger;
    }

    public async Task HandleEventAsync(StarGiftListedForResaleIntegrationEvent eventData)
    {
        var giftId = eventData.GiftId;
        
        _logger.LogInformation("[UpdateCatalogResaleCount] Gift {GiftId} listed for resale at {Price} stars",
            giftId, eventData.ResaleStars);

        var collection = _mongoDatabase.GetCollection<BsonDocument>("AvailableStarGiftReadModel");
        var filter = Builders<BsonDocument>.Filter.Eq("GiftId", giftId);

        // Берём текущую запись подарка
        var doc = await collection.Find(filter).FirstOrDefaultAsync();
        if (doc == null)
        {
            _logger.LogWarning("[UpdateCatalogResaleCount] Gift {GiftId} not found in catalog!", giftId);
            return;
        }

        // Читаем текущее значение, учитывая возможный null
        var currentValue = doc.Contains("AvailabilityResale") && !doc["AvailabilityResale"].IsBsonNull
            ? doc["AvailabilityResale"].ToInt64()
            : 0L;

        // Увеличиваем счётчик выставленных на перепродажу
        var update = Builders<BsonDocument>.Update.Set("AvailabilityResale", currentValue + 1);
        var result = await collection.UpdateOneAsync(filter, update);

        if (result.ModifiedCount > 0)
        {
            _logger.LogInformation("[UpdateCatalogResaleCount] Set AvailabilityResale to {Count} for gift {GiftId}",
                currentValue + 1, giftId);
        }
    }

    public async Task HandleEventAsync(StarGiftRemovedFromResaleIntegrationEvent eventData)
    {
        var giftId = eventData.GiftId;
        
        _logger.LogInformation("[UpdateCatalogResaleCount] Gift {GiftId} removed from resale", giftId);

        var collection = _mongoDatabase.GetCollection<BsonDocument>("AvailableStarGiftReadModel");
        var filter = Builders<BsonDocument>.Filter.Eq("GiftId", giftId);

        // Перед уменьшением убеждаемся, что поле есть и оно числовое
        var doc = await collection.Find(filter).FirstOrDefaultAsync();
        if (doc != null)
        {
            var currentValue = doc.Contains("AvailabilityResale") && !doc["AvailabilityResale"].IsBsonNull
                ? doc["AvailabilityResale"].ToInt64()
                : 0L;

            // Уменьшаем счётчик только если он больше нуля
            if (currentValue > 0)
            {
                var update = Builders<BsonDocument>.Update.Inc("AvailabilityResale", -1L);
                var result = await collection.UpdateOneAsync(filter, update);

                if (result.ModifiedCount > 0)
                {
                    _logger.LogInformation("[UpdateCatalogResaleCount] Decremented AvailabilityResale for gift {GiftId}", giftId);
                }
            }
            else
            {
                _logger.LogWarning("[UpdateCatalogResaleCount] AvailabilityResale already at 0 for gift {GiftId}", giftId);
            }
        }
        else
        {
            _logger.LogWarning("[UpdateCatalogResaleCount] Gift {GiftId} not found in catalog!", giftId);
        }
    }
}