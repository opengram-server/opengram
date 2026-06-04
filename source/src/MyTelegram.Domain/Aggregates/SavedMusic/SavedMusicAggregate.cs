using MyTelegram.Domain.Aggregates.SavedMusic.Events;

namespace MyTelegram.Domain.Aggregates.SavedMusic;

public class SavedMusicAggregate : AggregateRoot<SavedMusicAggregate, SavedMusicId>
{
    private readonly SavedMusicState _state = new();

    public SavedMusicAggregate(SavedMusicId id) : base(id)
    {
        Register(_state);
    }

    public void Create(long userId)
    {
        Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new SavedMusicCreatedEvent(userId));
    }

    public void AddMusic(long documentId, long? afterDocumentId, RequestInfo requestInfo)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        // If document already exists and afterDocumentId is specified, it's a reorder
        if (_state.DocumentIds.Contains(documentId) && afterDocumentId.HasValue)
        {
            Emit(new MusicReorderedEvent(_state.UserId, documentId, afterDocumentId.Value, requestInfo));
        }
        else
        {
            Emit(new MusicAddedEvent(_state.UserId, documentId, afterDocumentId, requestInfo));
        }
    }

    public void RemoveMusic(long documentId, RequestInfo requestInfo)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        if (!_state.DocumentIds.Contains(documentId))
        {
            return; // Already removed
        }
        
        Emit(new MusicRemovedEvent(_state.UserId, documentId, requestInfo));
    }
}
