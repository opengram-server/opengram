namespace MyTelegram.Domain.Aggregates.Photo;

public class PhotoState : AggregateState<PhotoAggregate, PhotoId, PhotoState>,
    IApply<PhotoCreatedEvent>,
    IApply<PhotoDeletedEvent>,
    IApply<SetAsProfilePhotoCompletedEvent>
{
    public long UserId { get; private set; }
    public PhotoItem Photo { get; private set; } = null!;

    public void Apply(PhotoCreatedEvent aggregateEvent)
    {
        UserId = aggregateEvent.UserId;
        Photo = aggregateEvent.Photo;
    }

    public void Apply(PhotoDeletedEvent aggregateEvent)
    {
        
    }

    public void Apply(SetAsProfilePhotoCompletedEvent aggregateEvent)
    {
        
    }
}