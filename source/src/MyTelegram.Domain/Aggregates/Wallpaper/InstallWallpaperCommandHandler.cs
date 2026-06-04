namespace MyTelegram.Domain.Aggregates.Wallpaper;

public class InstallWallpaperCommandHandler : CommandHandler<WallpaperAggregate, WallpaperId, InstallWallpaperCommand>
{
    public override Task ExecuteAsync(WallpaperAggregate aggregate, InstallWallpaperCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Install(
            command.RequestInfo,
            command.UserId,
            command.Settings!
        );
        return Task.CompletedTask;
    }
}
