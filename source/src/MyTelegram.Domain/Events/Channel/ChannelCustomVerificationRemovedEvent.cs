namespace MyTelegram.Domain.Events.Channel;

public class ChannelCustomVerificationRemovedEvent : RequestAggregateEvent2<ChannelAggregate, ChannelId>
{
    public ChannelCustomVerificationRemovedEvent(
        RequestInfo requestInfo,
        long channelId) : base(requestInfo)
    {
        ChannelId = channelId;
    }

    public long ChannelId { get; }
}
