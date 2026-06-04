using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MyTelegram.Domain.Aggregates.SavedMusic;
using MyTelegram.Domain.Aggregates.SavedMusic.Events;

namespace MyTelegram.ReadModel.Impl;

public class SavedMusicReadModel : ISavedMusicReadModel,
    IAmReadModelFor<SavedMusicAggregate, SavedMusicId, SavedMusicCreatedEvent>,
    IAmReadModelFor<SavedMusicAggregate, SavedMusicId, MusicAddedEvent>,
    IAmReadModelFor<SavedMusicAggregate, SavedMusicId, MusicRemovedEvent>,
    IAmReadModelFor<SavedMusicAggregate, SavedMusicId, MusicReorderedEvent>
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public int Version { get; set; }
    public long UserId { get; set; }
    public List<long> DocumentIds { get; set; } = new();

    public Task ApplyAsync(
        IReadModelContext context,
        IDomainEvent<SavedMusicAggregate, SavedMusicId, SavedMusicCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        UserId = domainEvent.AggregateEvent.UserId;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(
        IReadModelContext context,
        IDomainEvent<SavedMusicAggregate, SavedMusicId, MusicAddedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        
        // Remove if already exists (for reordering)
        DocumentIds.Remove(evt.DocumentId);
        
        if (evt.AfterDocumentId.HasValue)
        {
            var afterIndex = DocumentIds.IndexOf(evt.AfterDocumentId.Value);
            if (afterIndex >= 0)
            {
                DocumentIds.Insert(afterIndex + 1, evt.DocumentId);
            }
            else
            {
                DocumentIds.Insert(0, evt.DocumentId);
            }
        }
        else
        {
            DocumentIds.Insert(0, evt.DocumentId);
        }
        
        return Task.CompletedTask;
    }

    public Task ApplyAsync(
        IReadModelContext context,
        IDomainEvent<SavedMusicAggregate, SavedMusicId, MusicRemovedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        DocumentIds.Remove(domainEvent.AggregateEvent.DocumentId);
        return Task.CompletedTask;
    }

    public Task ApplyAsync(
        IReadModelContext context,
        IDomainEvent<SavedMusicAggregate, SavedMusicId, MusicReorderedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        
        DocumentIds.Remove(evt.DocumentId);
        
        var afterIndex = DocumentIds.IndexOf(evt.AfterDocumentId);
        if (afterIndex >= 0)
        {
            DocumentIds.Insert(afterIndex + 1, evt.DocumentId);
        }
        else
        {
            DocumentIds.Insert(0, evt.DocumentId);
        }
        
        return Task.CompletedTask;
    }
}
