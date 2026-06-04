namespace MyTelegram.Domain.Events.Channel;

public class ChannelParticipantCountChangedEvent(long channelId, int newParticipantCount) : AggregateEvent<ChannelAggregate, ChannelId>
{
    public long ChannelId { get; } = channelId;
    public int NewParticipantCount { get; } = newParticipantCount;
}