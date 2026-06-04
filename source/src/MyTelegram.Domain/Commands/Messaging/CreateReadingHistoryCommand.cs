namespace MyTelegram.Domain.Commands.Messaging;

public class CreateReadingHistoryCommand(
    ReadingHistoryId aggregateId,
    long readerPeerId,
    long targetPeerId,
    int messageId,
    int date)
    : Command<ReadingHistoryAggregate, ReadingHistoryId, IExecutionResult>(aggregateId)
{
    public long ReaderPeerId { get; } = readerPeerId;
    public long TargetPeerId { get; } = targetPeerId;
    public int MessageId { get; } = messageId;
    public int Date { get; } = date;

    /*RequestInfo requestInfo,*/
}