using MyTelegram.Domain.Aggregates.SavedMusic.Events;

namespace MyTelegram.Domain.Aggregates.SavedMusic;

public class SavedMusicState : AggregateState<SavedMusicAggregate, SavedMusicId, SavedMusicState>,
    IApply<SavedMusicCreatedEvent>,
    IApply<MusicAddedEvent>,
    IApply<MusicRemovedEvent>,
    IApply<MusicReorderedEvent>
{
    public long UserId { get; private set; }
    public List<long> DocumentIds { get; private set; } = new();

    public void Apply(SavedMusicCreatedEvent aggregateEvent)
    {
        UserId = aggregateEvent.UserId;
    }

    public void Apply(MusicAddedEvent aggregateEvent)
    {
        // Remove if already exists (for reordering)
        DocumentIds.Remove(aggregateEvent.DocumentId);
        
        if (aggregateEvent.AfterDocumentId.HasValue)
        {
            var afterIndex = DocumentIds.IndexOf(aggregateEvent.AfterDocumentId.Value);
            if (afterIndex >= 0)
            {
                DocumentIds.Insert(afterIndex + 1, aggregateEvent.DocumentId);
            }
            else
            {
                // If after_id not found, add to top
                DocumentIds.Insert(0, aggregateEvent.DocumentId);
            }
        }
        else
        {
            // Add to top
            DocumentIds.Insert(0, aggregateEvent.DocumentId);
        }
    }

    public void Apply(MusicRemovedEvent aggregateEvent)
    {
        DocumentIds.Remove(aggregateEvent.DocumentId);
    }

    public void Apply(MusicReorderedEvent aggregateEvent)
    {
        DocumentIds.Remove(aggregateEvent.DocumentId);
        
        var afterIndex = DocumentIds.IndexOf(aggregateEvent.AfterDocumentId);
        if (afterIndex >= 0)
        {
            DocumentIds.Insert(afterIndex + 1, aggregateEvent.DocumentId);
        }
        else
        {
            DocumentIds.Insert(0, aggregateEvent.DocumentId);
        }
    }
}
