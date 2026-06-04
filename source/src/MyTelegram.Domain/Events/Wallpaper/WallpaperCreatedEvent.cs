using MyTelegram.Domain.Aggregates.Wallpaper;

namespace MyTelegram.Domain.Events.Wallpaper;

public class WallpaperCreatedEvent(
    RequestInfo requestInfo,
    long wallpaperId,
    long accessHash,
    long creatorId,
    string slug,
    long? documentId,
    bool isPattern,
    bool isDark,
    bool isDefault,
    WallPaperSettings? settings,
    bool forChat) : RequestAggregateEvent2<WallpaperAggregate, WallpaperId>(requestInfo)
{
    public long WallpaperId { get; } = wallpaperId;
    public long AccessHash { get; } = accessHash;
    public long CreatorId { get; } = creatorId;
    public string Slug { get; } = slug;
    public long? DocumentId { get; } = documentId;
    public bool IsPattern { get; } = isPattern;
    public bool IsDark { get; } = isDark;
    public bool IsDefault { get; } = isDefault;
    public WallPaperSettings? Settings { get; } = settings;
    public bool ForChat { get; } = forChat;
}
