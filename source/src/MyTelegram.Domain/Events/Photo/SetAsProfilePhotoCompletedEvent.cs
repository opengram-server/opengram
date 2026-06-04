namespace MyTelegram.Domain.Events.Photo;

public class SetAsProfilePhotoCompletedEvent(long photoId) : AggregateEvent<PhotoAggregate, PhotoId>
{
    public long PhotoId { get; } = photoId;
}