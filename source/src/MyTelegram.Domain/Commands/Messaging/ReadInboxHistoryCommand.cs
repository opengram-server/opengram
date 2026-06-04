namespace MyTelegram.Domain.Commands.Messaging;

public class ReadInboxHistoryCommand(
    MessageId aggregateId,
    RequestInfo requestInfo,
    long readerUserId,
    int date)
    : Command<MessageAggregate, MessageId, IExecutionResult>(aggregateId), IHasRequestInfo
{
    public long ReaderUserId { get; } = readerUserId;
    public int Date { get; } = date;

    public RequestInfo RequestInfo { get; } = requestInfo;
}