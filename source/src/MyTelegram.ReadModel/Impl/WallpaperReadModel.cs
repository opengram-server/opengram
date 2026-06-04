using MyTelegram.Schema.Extensions;
using Newtonsoft.Json;
using System.Text;

using MyTelegram.Domain.Aggregates.Wallpaper;
using MyTelegram.Domain.Events.Wallpaper;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.ReadModel.Impl;

public class WallpaperReadModel : IWallpaperReadModel,
    IAmReadModelFor<WallpaperAggregate, WallpaperId, WallpaperCreatedEvent>,
    IAmReadModelFor<WallpaperAggregate, WallpaperId, WallpaperSavedEvent>,
    IAmReadModelFor<WallpaperAggregate, WallpaperId, WallpaperInstalledEvent>,
    IAmReadModelFor<WallpaperAggregate, WallpaperId, WallpaperDeletedEvent>
{
    public string Id { get; set; } = null!;
    public long WallpaperId { get; set; }
    public long AccessHash { get; set; }
    public long UserId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public long CreatorId { get; set; }
    public long? DocumentId { get; set; }
    public bool Pattern { get; set; }
    public bool Dark { get; set; }
    public bool Default { get; set; }
    public bool IsDeleted { get; set; }
    public bool ForChat { get; set; }
    private byte[]? _settings;
    public WallPaperSettings? Settings => _settings != null ? JsonConvert.DeserializeObject<WallPaperSettings>(Encoding.UTF8.GetString(_settings)) : null;

    public long? Version { get; set; }

    public Task ApplyAsync(IReadModelContext context, 
        IDomainEvent<WallpaperAggregate, WallpaperId, WallpaperCreatedEvent> domainEvent, 
        CancellationToken cancellationToken)
    {
        Id = domainEvent.AggregateIdentity.Value;
        WallpaperId = domainEvent.AggregateEvent.WallpaperId;
        AccessHash = domainEvent.AggregateEvent.AccessHash;
        CreatorId = domainEvent.AggregateEvent.CreatorId;
        Slug = domainEvent.AggregateEvent.Slug;
        DocumentId = domainEvent.AggregateEvent.DocumentId;
        Pattern = domainEvent.AggregateEvent.IsPattern;
        Dark = domainEvent.AggregateEvent.IsDark;
        Default = domainEvent.AggregateEvent.IsDefault;
        ForChat = domainEvent.AggregateEvent.ForChat;
        _settings = domainEvent.AggregateEvent.Settings != null ? Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(domainEvent.AggregateEvent.Settings)) : null;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, 
        IDomainEvent<WallpaperAggregate, WallpaperId, WallpaperSavedEvent> domainEvent, 
        CancellationToken cancellationToken)
    {
        _settings = domainEvent.AggregateEvent.Settings != null ? Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(domainEvent.AggregateEvent.Settings)) : null;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, 
        IDomainEvent<WallpaperAggregate, WallpaperId, WallpaperInstalledEvent> domainEvent, 
        CancellationToken cancellationToken)
    {
        _settings = domainEvent.AggregateEvent.Settings != null ? Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(domainEvent.AggregateEvent.Settings)) : null;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, 
        IDomainEvent<WallpaperAggregate, WallpaperId, WallpaperDeletedEvent> domainEvent, 
        CancellationToken cancellationToken)
    {
        IsDeleted = true;
        return Task.CompletedTask;
    }
}
