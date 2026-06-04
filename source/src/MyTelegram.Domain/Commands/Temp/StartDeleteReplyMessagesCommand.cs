namespace MyTelegram.Domain.Commands.Temp;

public class StartDeleteReplyMessagesCommand(
    TempId aggregateId,
    RequestInfo requestInfo,
    long channelId,
    List<int> messageIds,
    int newTopMessageId)
    : RequestCommand2<TempAggregate, TempId, IExecutionResult>(aggregateId, requestInfo)
{
    public long ChannelId { get; } = channelId;
    public List<int> MessageIds { get; } = messageIds;
    public int NewTopMessageId { get; } = newTopMessageId;
}