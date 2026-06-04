using EventFlow.Aggregates;
using EventFlow.Subscribers;
using MyTelegram.Domain.Aggregates.Messaging;
using MyTelegram.Domain.Events.Messaging;
using MyTelegram.Domain.Extensions;

namespace MyTelegram.Messenger.CommandServer.EventHandlers;

/// <summary>
/// Event handler that logs when scheduled messages are created
/// </summary>
public class ScheduledMessageCreatedEventHandler(
    ILogger<ScheduledMessageCreatedEventHandler> logger)
    : ISubscribeSynchronousTo<MessageAggregate, MessageId, OutboxMessageCreatedEvent>
{
    public Task HandleAsync(IDomainEvent<MessageAggregate, MessageId, OutboxMessageCreatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        var messageItem = evt.OutboxMessageItem;
        
        // Only log if this is a scheduled message
        if (messageItem.ScheduleDate.HasValue && messageItem.ScheduleMessageId.HasValue)
        {
            var currentTime = DateTime.UtcNow.ToTimestamp();
            
            logger.LogWarning(
                "*** SCHEDULED MESSAGE CREATED: ScheduleDate={ScheduleDate}, CurrentTime={CurrentTime}, Diff={Diff}s, UserId={UserId}, MessageId={MessageId} ***",
                messageItem.ScheduleDate.Value,
                currentTime,
                messageItem.ScheduleDate.Value - currentTime,
                messageItem.SenderUserId,
                messageItem.ScheduleMessageId.Value);
        }
        
        return Task.CompletedTask;
    }
}
