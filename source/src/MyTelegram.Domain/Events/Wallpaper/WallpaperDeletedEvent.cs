using MyTelegram.Domain.Aggregates.Wallpaper;

namespace MyTelegram.Domain.Events.Wallpaper;

public class WallpaperDeletedEvent(
    RequestInfo requestInfo,
    long wallpaperId,
    long userId) : RequestAggregateEvent2<WallpaperAggregate, WallpaperId>(requestInfo)
{
    public long WallpaperId { get; } = wallpaperId;
    public long UserId { get; } = userId;
}
