using MyTelegram.Domain.Aggregates.Wallpaper;

namespace MyTelegram.Domain.Events.Wallpaper;

public class WallpaperInstalledEvent(
    RequestInfo requestInfo,
    long wallpaperId,
    long userId,
    WallPaperSettings settings) : RequestAggregateEvent2<WallpaperAggregate, WallpaperId>(requestInfo)
{
    public long WallpaperId { get; } = wallpaperId;
    public long UserId { get; } = userId;
    public WallPaperSettings Settings { get; } = settings;
}
