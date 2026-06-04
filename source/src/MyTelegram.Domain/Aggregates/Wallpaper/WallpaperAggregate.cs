using MyTelegram.Domain.Events.Wallpaper;

namespace MyTelegram.Domain.Aggregates.Wallpaper;

public class WallpaperAggregate : MyInMemorySnapshotAggregateRoot<WallpaperAggregate, WallpaperId, WallpaperSnapshot>
{
    private readonly WallpaperState _state = new();

    public WallpaperAggregate(WallpaperId id) : base(id, SnapshotEveryFewVersionsStrategy.Default)
    {
        Register(_state);
    }

    public void Create(RequestInfo requestInfo, long wallpaperId, long accessHash, long creatorId, string slug, 
        long? documentId, bool isPattern, bool isDark, bool isDefault, WallPaperSettings? settings, bool forChat)
    {
        if (IsNew)
        {
            Emit(new WallpaperCreatedEvent(
                requestInfo,
                wallpaperId,
                accessHash,
                creatorId,
                slug,
                documentId,
                isPattern,
                isDark,
                isDefault,
                settings,
                forChat
            ));
        }
    }

    public void Save(RequestInfo requestInfo, long userId, bool unsave, WallPaperSettings settings)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        Emit(new WallpaperSavedEvent(
            requestInfo,
            _state.WallpaperId,
            userId,
            unsave,
            settings
        ));
    }

    public void Install(RequestInfo requestInfo, long userId, WallPaperSettings settings)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        Emit(new WallpaperInstalledEvent(
            requestInfo,
            _state.WallpaperId,
            userId,
            settings
        ));
    }

    public void Delete(RequestInfo requestInfo, long userId)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        Emit(new WallpaperDeletedEvent(
            requestInfo,
            _state.WallpaperId,
            userId
        ));
    }

    protected override Task<WallpaperSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_state.ToSnapshot());
    }

    protected override Task LoadSnapshotAsync(WallpaperSnapshot snapshot, ISnapshotMetadata metadata, CancellationToken cancellationToken)
    {
        _state.LoadFromSnapshot(snapshot);
        return Task.CompletedTask;
    }
}
