namespace MyTelegram.Domain.Events.Channel;

public class ChannelAboutEditedEvent(
    RequestInfo requestInfo,
    long channelId,
    string? about)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public string? About { get; } = about;
}