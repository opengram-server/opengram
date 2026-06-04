namespace MyTelegram.Domain.Events.Photo;

public class PhotoCreatedEvent(long userId, PhotoItem photo) : AggregateEvent<PhotoAggregate, PhotoId>
{
    public PhotoItem Photo { get; } = photo;
    public long UserId { get; } = userId;
}