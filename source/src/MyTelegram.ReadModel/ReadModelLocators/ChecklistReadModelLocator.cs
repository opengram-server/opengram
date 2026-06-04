using MyTelegram.Domain.Aggregates.Checklist;
using MyTelegram.Domain.Events.Checklist;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.ReadModel.ReadModelLocators;

public class ChecklistReadModelLocator : IReadModelLocator
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var aggregateEvent = domainEvent as IDomainEvent<ChecklistAggregate, ChecklistId, ChecklistCreatedEvent>;
        if (aggregateEvent != null)
        {
            yield return aggregateEvent.AggregateIdentity.Value;
        }
        
        var toggleEvent = domainEvent as IDomainEvent<ChecklistAggregate, ChecklistId, ChecklistTaskToggledEvent>;
        if (toggleEvent != null)
        {
            yield return toggleEvent.AggregateIdentity.Value;
        }

        var addEvent = domainEvent as IDomainEvent<ChecklistAggregate, ChecklistId, ChecklistTasksAddedEvent>;
        if (addEvent != null)
        {
            yield return addEvent.AggregateIdentity.Value;
        }

        var deleteEvent = domainEvent as IDomainEvent<ChecklistAggregate, ChecklistId, ChecklistDeletedEvent>;
        if (deleteEvent != null)
        {
            yield return deleteEvent.AggregateIdentity.Value;
        }
    }
}
