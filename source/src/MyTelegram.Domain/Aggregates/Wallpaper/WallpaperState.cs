using MyTelegram.Domain.Events.Wallpaper;

namespace MyTelegram.Domain.Aggregates.Wallpaper;

public class WallpaperState : AggregateState<WallpaperAggregate, WallpaperId, WallpaperState>,
    IApply<WallpaperCreatedEvent>,
    IApply<WallpaperSavedEvent>,
    IApply<WallpaperInstalledEvent>,
    IApply<WallpaperDeletedEvent>
{
    public long WallpaperId { get; private set; }
    public long AccessHash { get; private set; }
    public long CreatorId { get; private set; }
    public string Slug { get; private set; } = string.Empty;
    public long? DocumentId { get; private set; }
    public bool IsPattern { get; private set; }
    public bool IsDark { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsDeleted { get; private set; }
    public bool ForChat { get; private set; }
    public WallPaperSettings? Settings { get; private set; }

    public void Apply(WallpaperCreatedEvent aggregateEvent)
    {
        WallpaperId = aggregateEvent.WallpaperId;
        AccessHash = aggregateEvent.AccessHash;
        CreatorId = aggregateEvent.CreatorId;
        Slug = aggregateEvent.Slug;
        DocumentId = aggregateEvent.DocumentId;
        IsPattern = aggregateEvent.IsPattern;
        IsDark = aggregateEvent.IsDark;
        IsDefault = aggregateEvent.IsDefault;
        Settings = aggregateEvent.Settings;
        ForChat = aggregateEvent.ForChat;
    }

    public void Apply(WallpaperSavedEvent aggregateEvent)
    {
        if (aggregateEvent.Settings != null)
        {
            Settings = aggregateEvent.Settings;
        }
    }

    public void Apply(WallpaperInstalledEvent aggregateEvent)
    {
        if (aggregateEvent.Settings != null)
        {
            Settings = aggregateEvent.Settings;
        }
    }

    public void Apply(WallpaperDeletedEvent aggregateEvent)
    {
        IsDeleted = true;
    }

    public WallpaperSnapshot ToSnapshot()
    {
        return new WallpaperSnapshot(
            WallpaperId,
            AccessHash,
            CreatorId,
            Slug,
            DocumentId,
            IsPattern,
            IsDark,
            IsDefault,
            IsDeleted,
            ForChat,
            Settings
        );
    }

    public void LoadFromSnapshot(WallpaperSnapshot snapshot)
    {
        WallpaperId = snapshot.WallpaperId;
        AccessHash = snapshot.AccessHash;
        CreatorId = snapshot.CreatorId;
        Slug = snapshot.Slug;
        DocumentId = snapshot.DocumentId;
        IsPattern = snapshot.IsPattern;
        IsDark = snapshot.IsDark;
        IsDefault = snapshot.IsDefault;
        IsDeleted = snapshot.IsDeleted;
        ForChat = snapshot.ForChat;
        Settings = snapshot.Settings;
    }
}
