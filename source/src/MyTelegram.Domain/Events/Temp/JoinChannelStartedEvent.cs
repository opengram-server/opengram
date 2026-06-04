namespace MyTelegram.Domain.Events.Temp;

public class JoinChannelStartedEvent(RequestInfo requestInfo, long channelId, bool broadcast, int topMessageId, int channelHistoryMinId) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public bool Broadcast { get; } = broadcast;
    public int TopMessageId { get; } = topMessageId;
    public int ChannelHistoryMinId { get; } = channelHistoryMinId;
}