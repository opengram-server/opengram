using MongoDB.Driver;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.StarGift;

public class GetAvailableStarGiftsQueryHandler : IQueryHandler<GetAvailableStarGiftsQuery, IReadOnlyCollection<IAvailableStarGiftReadModel>>
{
    private readonly IMongoDatabase _database;

    public GetAvailableStarGiftsQueryHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<IReadOnlyCollection<IAvailableStarGiftReadModel>> ExecuteQueryAsync(GetAvailableStarGiftsQuery query, CancellationToken cancellationToken)
    {
        // Load gifts from MongoDB AvailableStarGiftReadModel collection (created by admin panel)
        var collection = _database.GetCollection<StarGiftCatalogReadModel>("AvailableStarGiftReadModel");
        
        // Show ALL gifts, including sold out ones (SoldOut flag will be visible in UI)
        var gifts = await collection.Find(_ => true).ToListAsync(cancellationToken);
        
        // Log count for debugging
        // Console.WriteLine($"[GetAvailableStarGiftsQueryHandler] Found {gifts.Count} gifts in DB (AvailableStarGiftReadModel)");
        
        return gifts.Cast<IAvailableStarGiftReadModel>().ToList();
    }
}

// ReadModel for AvailableStarGiftReadModel collection (catalog of available gifts, created by admin panel)
// Admin panel uses PascalCase field names
public class StarGiftCatalogReadModel : IAvailableStarGiftReadModel
{
    public long GiftId { get; set; }
    public bool Limited { get; set; }
    public bool SoldOut { get; set; }
    public bool Birthday { get; set; }
    public bool RequirePremium { get; set; }
    public bool LimitedPerUser { get; set; }
    public long? Sticker { get; set; }
    public long Stars { get; set; }
    public int? AvailabilityRemains { get; set; }
    public int? AvailabilityTotal { get; set; }
    public long? AvailabilityResale { get; set; }
    public long ConvertStars { get; set; }
    public int? FirstSaleDate { get; set; }
    public int? LastSaleDate { get; set; }
    public long? UpgradeStars { get; set; }
    public long? ResellMinStars { get; set; }
    public string? Title { get; set; }
    public byte[]? ReleasedBy { get; set; }
    public int? PerUserTotal { get; set; }
    public int? PerUserRemains { get; set; }
    public int? LockedUntilDate { get; set; }
    public long? Version { get; set; }
}
