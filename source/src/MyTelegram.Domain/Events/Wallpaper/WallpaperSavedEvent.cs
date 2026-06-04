using MyTelegram.Domain.Aggregates.Wallpaper;

namespace MyTelegram.Domain.Events.Wallpaper;

public class WallpaperSavedEvent(
    RequestInfo requestInfo,
    long wallpaperId,
    long userId,
    bool unsave,
    WallPaperSettings? settings) : RequestAggregateEvent2<WallpaperAggregate, WallpaperId>(requestInfo)
{
    public long WallpaperId { get; } = wallpaperId;
    public long UserId { get; } = userId;
    public bool Unsave { get; } = unsave;
    public WallPaperSettings? Settings { get; } = settings;
}
