using MyTelegram.Domain.Aggregates.Wallpaper;
using MyTelegram.Domain.Events.Wallpaper;

namespace MyTelegram.ReadModel.ReadModelLocators;

public class WallpaperReadModelLocator : IReadModelLocator
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case IDomainEvent<WallpaperAggregate, WallpaperId, WallpaperCreatedEvent> e:
                yield return e.AggregateIdentity.Value;
                break;
            case IDomainEvent<WallpaperAggregate, WallpaperId, WallpaperSavedEvent> e:
                yield return e.AggregateIdentity.Value;
                break;
            case IDomainEvent<WallpaperAggregate, WallpaperId, WallpaperInstalledEvent> e:
                yield return e.AggregateIdentity.Value;
                break;
            case IDomainEvent<WallpaperAggregate, WallpaperId, WallpaperDeletedEvent> e:
                yield return e.AggregateIdentity.Value;
                break;
        }
    }
}
