namespace MyTelegram.Domain.Events.Messaging;

public class ReadingHistoryCreatedEvent(long readerPeerId, long targetPeerId, int messageId, int date)
    : AggregateEvent<ReadingHistoryAggregate, ReadingHistoryId>
{
    public long ReaderPeerId { get; } = readerPeerId;
    public long TargetPeerId { get; } = targetPeerId;
    public int MessageId { get; } = messageId;
    public int Date { get; } = date;
}