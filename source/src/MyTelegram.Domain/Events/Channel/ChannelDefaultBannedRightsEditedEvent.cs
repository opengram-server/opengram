namespace MyTelegram.Domain.Events.Channel;

public class ChannelDefaultBannedRightsEditedEvent(
    RequestInfo requestInfo,
    long channelId,
    ChatBannedRights defaultBannedRights)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public ChatBannedRights DefaultBannedRights { get; } = defaultBannedRights;
}
