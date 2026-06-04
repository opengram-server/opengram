namespace MyTelegram.Domain.Events.Channel;

public class ChatInviteRequestPendingUpdatedEvent(
    long channelId,
    List<long> channelAdmins,
    List<long> recentRequesters,
    int? requestsPending)
    : AggregateEvent<ChannelAggregate, ChannelId>
{
    public long ChannelId { get; } = channelId;
    public List<long> ChannelAdmins { get; } = channelAdmins;
    public List<long> RecentRequesters { get; } = recentRequesters;
    public int? RequestsPending { get; } = requestsPending;
}