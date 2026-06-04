namespace MyTelegram.Domain.Aggregates.Messaging;

public class MessageViewLogAggregate(MessageViewLogId id)
    : AggregateRoot<MessageViewLogAggregate, MessageViewLogId>(id),
        IApply<CheckMessageViewLogSuccessEvent>
{
    public void Apply(CheckMessageViewLogSuccessEvent aggregateEvent)
    {
    }

    public void CheckMessageViewLog(
        RequestInfo requestInfo,
        int messageId)
    {
        Emit(new CheckMessageViewLogSuccessEvent(requestInfo, messageId, !IsNew));
    }
}