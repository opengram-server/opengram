namespace MyTelegram.Domain.Sagas;

public class UploadProfilePhotoSagaId(string value) : SingleValueObject<string>(value), ISagaId;

public class UploadProfilePhotoSagaLocator : DefaultSagaLocator<UploadProfilePhotoSaga, UploadProfilePhotoSagaId>
{
    protected override UploadProfilePhotoSagaId CreateSagaId(string requestId)
    {
        return new UploadProfilePhotoSagaId(requestId);
    }
}

public class UploadProfilePhotoSaga(UploadProfilePhotoSagaId id, IEventStore eventStore) : MyInMemoryAggregateSaga<UploadProfilePhotoSaga, UploadProfilePhotoSagaId, UploadProfilePhotoSagaLocator>(id, eventStore),
    ISagaIsStartedBy<UserAggregate, UserId, UserProfilePhotoUploadedEvent>
{
    public Task HandleAsync(IDomainEvent<UserAggregate, UserId, UserProfilePhotoUploadedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        //var command = new SetAsProfilePhotoCommand(PhotoId.Create(domainEvent.AggregateEvent.PhotoId));
        //Publish(command);

        return CompleteAsync(cancellationToken);
    }
}
