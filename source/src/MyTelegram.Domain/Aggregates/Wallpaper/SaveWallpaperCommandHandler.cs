namespace MyTelegram.Domain.Aggregates.Wallpaper;

public class SaveWallpaperCommandHandler : CommandHandler<WallpaperAggregate, WallpaperId, SaveWallpaperCommand>
{
    public override Task ExecuteAsync(WallpaperAggregate aggregate, SaveWallpaperCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Save(
            command.RequestInfo,
            command.UserId,
            command.Unsave,
            command.Settings!
        );
        return Task.CompletedTask;
    }
}
