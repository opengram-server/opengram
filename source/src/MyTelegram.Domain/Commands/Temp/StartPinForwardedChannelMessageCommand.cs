namespace MyTelegram.Domain.Commands.Temp;

public class StartPinForwardedChannelMessageCommand(
    TempId aggregateId,
    RequestInfo requestInfo,
    long channelId,
    int messageId)
    : RequestCommand2<TempAggregate, TempId, IExecutionResult>(aggregateId, requestInfo)
{
    public long ChannelId { get; } = channelId;
    public int MessageId { get; } = messageId;
}