using MyTelegram.Domain.Aggregates.Wallpaper;
using MyTelegram.Domain.Events.Wallpaper;

namespace MyTelegram.Domain.Commands.Wallpaper;

public class InstallWallpaperCommand(
    WallpaperId aggregateId,
    RequestInfo requestInfo,
    long userId,
    WallPaperSettings settings) : RequestCommand2<WallpaperAggregate, WallpaperId, IExecutionResult>(aggregateId, requestInfo)
{
    public long UserId { get; } = userId;
    public WallPaperSettings Settings { get; } = settings;
}
