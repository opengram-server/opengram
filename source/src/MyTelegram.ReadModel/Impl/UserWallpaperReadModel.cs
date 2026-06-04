using MyTelegram.Schema.Extensions;
using Newtonsoft.Json;
using System.Text;

using MyTelegram.Domain.Aggregates.Wallpaper;
using MyTelegram.Domain.Events.Wallpaper;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.Schema;

namespace MyTelegram.ReadModel.Impl;

/// <summary>
/// Stores user's saved and installed wallpapers
/// </summary>
public class UserWallpaperReadModel : IUserWallPaperReadModel,
    IAmReadModelFor<WallpaperAggregate, WallpaperId, WallpaperSavedEvent>,
    IAmReadModelFor<WallpaperAggregate, WallpaperId, WallpaperInstalledEvent>
{
    public string Id { get; private set; } = null!;
    public long UserId { get; private set; }
    public long WallPaperId { get; private set; }
    public bool IsSaved { get; private set; }
    public bool IsInstalled { get; private set; }
    private byte[]? _settings;
    public long? Version { get; set; }
    
    public WallPaperSettings? WallPaperSettings => _settings != null ? JsonConvert.DeserializeObject<WallPaperSettings>(Encoding.UTF8.GetString(_settings)) : null;


    public Task ApplyAsync(IReadModelContext context, 
        IDomainEvent<WallpaperAggregate, WallpaperId, WallpaperSavedEvent> domainEvent, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = $"{domainEvent.AggregateEvent.UserId}_{domainEvent.AggregateEvent.WallpaperId}";
            UserId = domainEvent.AggregateEvent.UserId;
            WallPaperId = domainEvent.AggregateEvent.WallpaperId;
        }

        IsSaved = !domainEvent.AggregateEvent.Unsave;
        _settings = domainEvent.AggregateEvent.Settings != null ? Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(domainEvent.AggregateEvent.Settings)) : null;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, 
        IDomainEvent<WallpaperAggregate, WallpaperId, WallpaperInstalledEvent> domainEvent, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = $"{domainEvent.AggregateEvent.UserId}_{domainEvent.AggregateEvent.WallpaperId}";
            UserId = domainEvent.AggregateEvent.UserId;
            WallPaperId = domainEvent.AggregateEvent.WallpaperId;
        }

        IsInstalled = true;
        _settings = domainEvent.AggregateEvent.Settings != null ? Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(domainEvent.AggregateEvent.Settings)) : null;

        return Task.CompletedTask;
    }
}
