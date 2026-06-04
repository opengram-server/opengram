using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Events.GroupCall;

namespace MyTelegram.ReadModel.ReadModelLocators;

public class GroupCallReadModelLocator : IReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        var aggregateEvent = domainEvent.GetAggregateEvent();
        
        // Check each event type explicitly
        long? callId = aggregateEvent switch
        {
            GroupCallCreatedEvent e => e.CallId,
            GroupCallParticipantJoinedEvent e => e.CallId,
            GroupCallParticipantLeftEvent e => e.CallId,
            GroupCallDiscardedEvent e => e.CallId,
            GroupCallSettingsUpdatedEvent e => e.CallId,
            GroupCallParticipantUpdatedEvent e => e.CallId,
            _ => null
        };
        
        if (callId.HasValue)
        {
            yield return GroupCallId.Create(callId.Value).Value;
        }
    }
}
