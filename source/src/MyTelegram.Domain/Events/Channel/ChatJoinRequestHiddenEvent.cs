namespace MyTelegram.Domain.Events.Channel;

public class ChatJoinRequestHiddenEvent(
    RequestInfo requestInfo,
    long channelId,
    bool isBroadcast,
    long userId,
    bool approved,
    int requestsPending,
    List<long> recentRequesters)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public bool IsBroadcast { get; } = isBroadcast;
    public long UserId { get; } = userId;
    public bool Approved { get; } = approved;
    public int RequestsPending { get; } = requestsPending;
    public List<long> RecentRequesters { get; } = recentRequesters;
}