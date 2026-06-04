namespace MyTelegram.Messenger.Services.Impl;

public class DomainEventMessageFactory(IEventJsonSerializer eventJsonSerializer) : IDomainEventMessageFactory, ITransientDependency
{
    public DomainEventMessage CreateDomainEventMessage(IDomainEvent domainEvent)
    {
        var serializedEvent = eventJsonSerializer.Serialize(
            domainEvent.GetAggregateEvent(),
            domainEvent.Metadata);

        return new DomainEventMessage(domainEvent.Metadata.EventId.Value, serializedEvent.SerializedData,
            domainEvent.Metadata);
    }
}