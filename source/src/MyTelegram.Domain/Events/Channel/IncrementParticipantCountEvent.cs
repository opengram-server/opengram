namespace MyTelegram.Domain.Events.Channel;

public class IncrementParticipantCountEvent(long channelId, int newParticipantCount)
    : AggregateEvent<ChannelAggregate, ChannelId> //, IHasCorrelationId
{
    public long ChannelId { get; } = channelId;
    public int NewParticipantCount { get; } = newParticipantCount;
}