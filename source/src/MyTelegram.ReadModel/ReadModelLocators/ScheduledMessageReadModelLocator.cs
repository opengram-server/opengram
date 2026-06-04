namespace MyTelegram.ReadModel.ReadModelLocators;

public class ScheduledMessageReadModelLocator : IScheduledMessageReadModelLocator, ITransientDependency
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        if (domainEvent is IDomainEvent<MessageAggregate, MessageId, OutboxMessageCreatedEvent> outboxMessageCreatedEvent)
        {
            var scheduleMessageId = outboxMessageCreatedEvent.AggregateEvent.OutboxMessageItem.ScheduleMessageId;
            if (scheduleMessageId.HasValue)
            {
                yield return ScheduledMessageId.Create(outboxMessageCreatedEvent.AggregateEvent.OutboxMessageItem.OwnerPeer.PeerId, scheduleMessageId.Value);
            }
        }
    }
}
