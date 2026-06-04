namespace MyTelegram.Domain.Events.Messaging;

public class MessageViewsIncrementedEvent(int messageId, int views) : AggregateEvent<MessageAggregate, MessageId>
{
    public int MessageId { get; } = messageId;
    public int Views { get; } = views;
}