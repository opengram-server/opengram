namespace MyTelegram.Domain.Aggregates.Wallpaper;

public class CreateWallpaperCommandHandler : CommandHandler<WallpaperAggregate, WallpaperId, CreateWallpaperCommand>
{
    public override Task ExecuteAsync(WallpaperAggregate aggregate, CreateWallpaperCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Create(
            command.RequestInfo,
            command.WallpaperId,
            command.AccessHash,
            command.UserId,
            command.Slug,
            command.DocumentId,
            command.IsPattern,
            command.IsDark,
            command.IsDefault,
            command.Settings,
            false
        );
        return Task.CompletedTask;
    }
}
