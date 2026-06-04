namespace MyTelegram.Domain.Events.User;

/// <summary>
/// Event fired when user installs a sticker set
/// </summary>
public class StickerSetInstalledEvent(
    RequestInfo requestInfo,
    long userId,
    long stickerSetId,
    bool archived,
    StickerSetType stickerSetType) 
    : RequestAggregateEvent2<UserAggregate, UserId>(requestInfo)
{
    public long UserId { get; } = userId;
    
    /// <summary>
    /// ID of the installed sticker set
    /// </summary>
    public long StickerSetId { get; } = stickerSetId;
    
    /// <summary>
    /// Whether the sticker set is archived
    /// </summary>
    public bool Archived { get; } = archived;
    
    /// <summary>
    /// Type of the sticker set (Regular, CustomEmoji, Mask, System)
    /// </summary>
    public StickerSetType StickerSetType { get; } = stickerSetType;
}
