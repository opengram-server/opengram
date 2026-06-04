namespace MyTelegram.Messenger.CommandServer.DomainEventHandlers;

public class QrCodeLoginDomainEventHandler(ICacheHelper<long, long> cacheHelper) : ISubscribeSynchronousTo<QrCodeAggregate, QrCodeId, LoginTokenAcceptedEvent>
{
    public Task HandleAsync(IDomainEvent<QrCodeAggregate, QrCodeId, LoginTokenAcceptedEvent> domainEvent, CancellationToken cancellationToken)
    {
        cacheHelper.TryAdd(
            domainEvent.AggregateEvent.QrCodeLoginRequestTempAuthKeyId, domainEvent.AggregateEvent.UserId);

        return Task.CompletedTask;
    }
}