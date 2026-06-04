namespace MyTelegram.Domain.Events.Photo;

public class PhotoDeletedEvent(long userId, PhotoItem photo) : AggregateEvent<PhotoAggregate, PhotoId>
{
    public long UserId { get; } = userId;
    public PhotoItem Photo { get; } = photo;
}