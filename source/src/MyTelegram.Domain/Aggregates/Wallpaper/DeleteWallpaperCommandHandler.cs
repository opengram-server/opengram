namespace MyTelegram.Domain.Aggregates.Wallpaper;

public class DeleteWallpaperCommandHandler : CommandHandler<WallpaperAggregate, WallpaperId, DeleteWallpaperCommand>
{
    public override Task ExecuteAsync(WallpaperAggregate aggregate, DeleteWallpaperCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Delete(
            command.RequestInfo,
            command.UserId
        );
        return Task.CompletedTask;
    }
}
