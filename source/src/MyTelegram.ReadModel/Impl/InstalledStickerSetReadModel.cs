using EventFlow.MongoDB.ReadStores.Attributes;
using MyTelegram.Domain.Extensions;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.ReadModel.Impl;

[MongoDbCollectionName("eventflow-installedstickersetreadmodel")]
public class InstalledStickerSetReadModel : IInstalledStickerSetReadModel,
    IAmReadModelFor<UserAggregate, UserId, StickerSetInstalledEvent>
{
    public virtual string Id { get; set; } = null!;
    public virtual long? Version { get; set; }
    public long UserId { get; set; }
    public long StickerSetId { get; set; }
    public bool Archived { get; set; }
    public StickerSetType StickerSetType { get; set; }
    public int Date { get; set; }

    public Task ApplyAsync(IReadModelContext context, 
        IDomainEvent<UserAggregate, UserId, StickerSetInstalledEvent> domainEvent, 
        CancellationToken cancellationToken)
    {
        UserId = domainEvent.AggregateEvent.UserId;
        StickerSetId = domainEvent.AggregateEvent.StickerSetId;
        Archived = domainEvent.AggregateEvent.Archived;
        Date = DateTime.UtcNow.ToTimestamp();
        StickerSetType = domainEvent.AggregateEvent.StickerSetType;
        
        return Task.CompletedTask;
    }
}
