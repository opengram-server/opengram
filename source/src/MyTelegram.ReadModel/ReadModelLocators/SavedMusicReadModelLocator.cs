using MyTelegram.Domain.Aggregates.SavedMusic;
using MyTelegram.Domain.Aggregates.SavedMusic.Events;

namespace MyTelegram.ReadModel.ReadModelLocators;

public class SavedMusicReadModelLocator : ISavedMusicReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case IDomainEvent<SavedMusicAggregate, SavedMusicId, SavedMusicCreatedEvent> e:
                yield return $"savedmusic-{e.AggregateEvent.UserId}";
                break;
            case IDomainEvent<SavedMusicAggregate, SavedMusicId, MusicAddedEvent> e:
                yield return $"savedmusic-{e.AggregateEvent.UserId}";
                break;
            case IDomainEvent<SavedMusicAggregate, SavedMusicId, MusicRemovedEvent> e:
                yield return $"savedmusic-{e.AggregateEvent.UserId}";
                break;
            case IDomainEvent<SavedMusicAggregate, SavedMusicId, MusicReorderedEvent> e:
                yield return $"savedmusic-{e.AggregateEvent.UserId}";
                break;
        }
    }
}
