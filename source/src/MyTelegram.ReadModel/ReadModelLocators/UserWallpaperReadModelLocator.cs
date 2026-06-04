using MyTelegram.Domain.Aggregates.Wallpaper;
using MyTelegram.Domain.Events.Wallpaper;

namespace MyTelegram.ReadModel.ReadModelLocators;

public class UserWallpaperReadModelLocator : IReadModelLocator
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case IDomainEvent<WallpaperAggregate, WallpaperId, WallpaperSavedEvent> e:
                yield return $"{e.AggregateEvent.UserId}_{e.AggregateEvent.WallpaperId}";
                break;
            case IDomainEvent<WallpaperAggregate, WallpaperId, WallpaperInstalledEvent> e:
                yield return $"{e.AggregateEvent.UserId}_{e.AggregateEvent.WallpaperId}";
                break;
        }
    }
}
