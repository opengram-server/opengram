namespace MyTelegram.Domain.Events.Channel;

public class ChannelParticipantsHiddenUpdatedEvent(RequestInfo requestInfo, long channelId, bool enabled) : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public bool Enabled { get; } = enabled;
}

//public class PendingJoinRequestCreatedEvent(RequestInfo requestInfo, long channelId, long requestUserId,
//    int requestsPending,
//    List<long> recentRequesters) : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
//{
//    public long ChannelId { get; } = channelId;
//    public long RequestUserId { get; } = requestUserId;
//    public int RequestsPending { get; } = requestsPending;
//    public List<long> RecentRequesters { get; } = recentRequesters;
//}
