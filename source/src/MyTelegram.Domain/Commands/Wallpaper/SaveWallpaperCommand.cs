using MyTelegram.Domain.Aggregates.Wallpaper;
using MyTelegram.Domain.Events.Wallpaper;

namespace MyTelegram.Domain.Commands.Wallpaper;

public class SaveWallpaperCommand(
    WallpaperId aggregateId,
    RequestInfo requestInfo,
    long userId,
    bool unsave,
    WallPaperSettings settings) : RequestCommand2<WallpaperAggregate, WallpaperId, IExecutionResult>(aggregateId, requestInfo)
{
    public long UserId { get; } = userId;
    public bool Unsave { get; } = unsave;
    public WallPaperSettings Settings { get; } = settings;
}
