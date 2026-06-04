namespace MyTelegram.ReadModel.Interfaces;

/// <summary>
/// Read model for Star Gifts
/// </summary>
public interface IStarGiftReadModel : IReadModel
{
    /// <summary>
    /// Unique identifier for this gift instance (format: {userId}_{messageId})
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Real aggregate ID in format "stargift-[GUID]"
    /// </summary>
    string AggregateId { get; }

    /// <summary>
    /// Star gift template ID
    /// </summary>
    long GiftId { get; }

    /// <summary>
    /// User ID who sent the gift
    /// </summary>
    long FromUserId { get; }

    /// <summary>
    /// User ID who received the gift
    /// </summary>
    long ToUserId { get; }

    /// <summary>
    /// Peer ID if sent to a channel
    /// </summary>
    long? ToPeerId { get; }

    /// <summary>
    /// Message ID containing the gift (inbox message for recipient)
    /// </summary>
    int MessageId { get; }

    /// <summary>
    /// Outbox message ID (for sender)
    /// </summary>
    int? OutboxMessageId { get; }

    /// <summary>
    /// Stars cost of the gift
    /// </summary>
    long Stars { get; }

    /// <summary>
    /// Stars received when converting this gift
    /// </summary>
    long ConvertStars { get; }

    /// <summary>
    /// Optional message from sender
    /// </summary>
    string? Message { get; }

    /// <summary>
    /// Whether sender name is hidden
    /// </summary>
    bool NameHidden { get; }

    /// <summary>
    /// Whether this gift is saved to profile
    /// </summary>
    bool Saved { get; }

    /// <summary>
    /// Whether this gift has been converted to Stars
    /// </summary>
    bool Converted { get; }

    /// <summary>
    /// Whether this gift has been upgraded to collectible
    /// </summary>
    bool Upgraded { get; }

    /// <summary>
    /// Whether this gift was refunded
    /// </summary>
    bool Refunded { get; }

    /// <summary>
    /// Whether this gift can be upgraded
    /// </summary>
    bool CanUpgrade { get; }

    /// <summary>
    /// Whether this gift is pinned on profile
    /// </summary>
    bool Pinned { get; }

    /// <summary>
    /// Unique saved ID for profile display
    /// </summary>
    long? SavedId { get; }

    /// <summary>
    /// Collectible ID (starGiftUnique.id) - used for emoji status
    /// Small unique number generated from UniqueId hash
    /// </summary>
    long? CollectibleId { get; }

    /// <summary>
    /// Cost to upgrade this gift
    /// </summary>
    long? UpgradeStars { get; }

    /// <summary>
    /// Message ID after upgrade
    /// </summary>
    int? UpgradeMsgId { get; }

    /// <summary>
    /// Date when gift was sent (Unix timestamp)
    /// </summary>
    int Date { get; }

    /// <summary>
    /// Date when gift was converted (Unix timestamp)
    /// </summary>
    int? ConvertDate { get; }

    /// <summary>
    /// Date when gift was upgraded (Unix timestamp)
    /// </summary>
    int? UpgradeDate { get; }

    /// <summary>
    /// Serialized gift sticker document
    /// </summary>
    byte[]? GiftSticker { get; }

    /// <summary>
    /// Sticker document ID for the gift
    /// </summary>
    long? StickerDocumentId { get; }

    /// <summary>
    /// Gift title/name
    /// </summary>
    string? Title { get; }

    /// <summary>
    /// Unique ID for upgraded gift (format: {aggregateId}_{timestamp})
    /// </summary>
    string? UniqueId { get; }

    /// <summary>
    /// Serialized upgrade attributes (model, pattern, backdrop)
    /// </summary>
    byte[]? Attributes { get; }
    
    /// <summary>
    /// Slug for unique gift (e.g., "PlushPepe-1234")
    /// </summary>
    string? UniqueSlug { get; }
    
    /// <summary>
    /// Sequential number for unique gift
    /// </summary>
    int? UniqueNum { get; }
    
    /// <summary>
    /// Whether this gift is listed for resale
    /// </summary>
    bool ForResale { get; }
    
    /// <summary>
    /// Resale price in Stars (if listed for resale)
    /// </summary>
    long? ResaleStars { get; }
    
    /// <summary>
    /// Date when gift was listed for resale (Unix timestamp)
    /// </summary>
    int? ResaleDate { get; }
    
    /// <summary>
    /// Date when gift was sold via resale (Unix timestamp)
    /// </summary>
    int? SoldDate { get; }
    
    /// <summary>
    /// User ID who bought this gift via resale
    /// </summary>
    long? BoughtByUserId { get; }
}
