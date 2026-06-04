namespace MyTelegram.Domain.Events.Channel;

public class ChannelDeletedEvent(RequestInfo requestInfo, long channelId, bool broadcast, bool megagroup, long accessHash, string title) : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public bool Broadcast { get; } = broadcast;
    public bool Megagroup { get; } = megagroup;
    public long AccessHash { get; } = accessHash;
    public string Title { get; } = title;
}