using MyTelegram.Domain.Aggregates.Wallpaper;
using MyTelegram.Domain.Events.Wallpaper;

namespace MyTelegram.Domain.Commands.Wallpaper;

public class DeleteWallpaperCommand(
    WallpaperId aggregateId,
    RequestInfo requestInfo,
    long userId) : RequestCommand2<WallpaperAggregate, WallpaperId, IExecutionResult>(aggregateId, requestInfo)
{
    public long UserId { get; } = userId;
}
