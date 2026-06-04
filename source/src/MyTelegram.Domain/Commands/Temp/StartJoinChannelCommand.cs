namespace MyTelegram.Domain.Commands.Temp;

public class StartJoinChannelCommand(TempId aggregateId, RequestInfo requestInfo, long channelId, bool broadcast, int topMessageId, int channelHistoryMinId) : RequestCommand2<TempAggregate, TempId, IExecutionResult>(aggregateId, requestInfo)
{
    public long ChannelId { get; } = channelId;
    public bool Broadcast { get; } = broadcast;
    public int TopMessageId { get; } = topMessageId;
    public int ChannelHistoryMinId { get; } = channelHistoryMinId;
}