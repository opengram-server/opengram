namespace MyTelegram.ReadModel.Interfaces;

/// <summary>
/// Read model for available Star Gift templates
/// </summary>
public interface IAvailableStarGiftReadModel : IReadModel
{
    /// <summary>
    /// Unique gift template ID
    /// </summary>
    long GiftId { get; }

    /// <summary>
    /// Whether this is a limited edition gift
    /// </summary>
    bool Limited { get; }

    /// <summary>
    /// Whether this gift is sold out
    /// </summary>
    bool SoldOut { get; }

    /// <summary>
    /// Whether this is a birthday gift
    /// </summary>
    bool Birthday { get; }

    /// <summary>
    /// Whether this gift requires premium
    /// </summary>
    bool RequirePremium { get; }

    /// <summary>
    /// Whether this gift has per-user purchase limit
    /// </summary>
    bool LimitedPerUser { get; }

    /// <summary>
    /// Sticker document ID for the gift (references DocumentReadModel)
    /// </summary>
    long? Sticker { get; }

    /// <summary>
    /// Cost in Telegram Stars
    /// </summary>
    long Stars { get; }

    /// <summary>
    /// Remaining availability count
    /// </summary>
    int? AvailabilityRemains { get; }

    /// <summary>
    /// Total availability count
    /// </summary>
    int? AvailabilityTotal { get; }

    /// <summary>
    /// Minimum resale price in Stars
    /// </summary>
    long? AvailabilityResale { get; }

    /// <summary>
    /// Stars received when converting
    /// </summary>
    long ConvertStars { get; }

    /// <summary>
    /// First sale date (Unix timestamp)
    /// </summary>
    int? FirstSaleDate { get; }

    /// <summary>
    /// Last sale date (Unix timestamp)
    /// </summary>
    int? LastSaleDate { get; }

    /// <summary>
    /// Cost to upgrade to collectible
    /// </summary>
    long? UpgradeStars { get; }

    /// <summary>
    /// Minimum resale price
    /// </summary>
    long? ResellMinStars { get; }

    /// <summary>
    /// Gift title
    /// </summary>
    string? Title { get; }

    /// <summary>
    /// Serialized peer who released this gift
    /// </summary>
    byte[]? ReleasedBy { get; }

    /// <summary>
    /// Total purchases allowed per user
    /// </summary>
    int? PerUserTotal { get; }

    /// <summary>
    /// Remaining purchases for current user
    /// </summary>
    int? PerUserRemains { get; }

    /// <summary>
    /// Locked until date (Unix timestamp)
    /// </summary>
    int? LockedUntilDate { get; }
}
