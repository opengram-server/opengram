using MongoDB.Bson.Serialization.Attributes;

namespace MyTelegram.ReadModel.Impl;

/// <summary>
/// Read-модель каталога доступных звёздных подарков.
/// Заполняется через админские операции и сидинг, а не доменными событиями.
/// Имя коллекции: AvailableStarGiftReadModel (регистрируется в QueryServer).
/// </summary>
[BsonIgnoreExtraElements]
public class AvailableStarGiftReadModel : IAvailableStarGiftReadModel
{
    public long GiftId { get; set; }
    public bool Limited { get; set; }
    public bool SoldOut { get; set; }
    public bool Birthday { get; set; }
    public bool RequirePremium { get; set; }
    public bool LimitedPerUser { get; set; }
    public long? Sticker { get; set; }  // Храним DocumentId, а не сериализованные байты
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
