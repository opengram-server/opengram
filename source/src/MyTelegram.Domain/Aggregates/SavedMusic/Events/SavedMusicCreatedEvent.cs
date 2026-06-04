namespace MyTelegram.Domain.Aggregates.SavedMusic.Events;

public class SavedMusicCreatedEvent(long userId) : AggregateEvent<SavedMusicAggregate, SavedMusicId>
{
    public long UserId { get; } = userId;
}
