using MyTelegram.Domain.Aggregates.Wallpaper;
using MyTelegram.Domain.Events.Wallpaper;

namespace MyTelegram.Domain.Commands.Wallpaper;

public class CreateWallpaperCommand(
    WallpaperId aggregateId,
    RequestInfo requestInfo,
    long creatorId,
    string slug,
    long? documentId,
    bool isPattern,
    bool isDark,
    bool isDefault,
    WallPaperSettings? settings,
    bool forChat) : RequestCommand2<WallpaperAggregate, WallpaperId, IExecutionResult>(aggregateId, requestInfo)
{
    public long CreatorId { get; } = creatorId;
    public string Slug { get; } = slug;
    public long? DocumentId { get; } = documentId;
    public bool IsPattern { get; } = isPattern;
    public bool IsDark { get; } = isDark;
    public bool IsDefault { get; } = isDefault;
    public WallPaperSettings? Settings { get; } = settings;
    public bool ForChat { get; } = forChat;
}
