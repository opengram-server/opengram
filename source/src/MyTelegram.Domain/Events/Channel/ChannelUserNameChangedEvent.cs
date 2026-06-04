namespace MyTelegram.Domain.Events.Channel;

public class ChannelUserNameChangedEvent(
    RequestInfo requestInfo,
    long channelId,
    string userName,
    string? oldUserName)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public string? OldUserName { get; } = oldUserName;
    public string UserName { get; } = userName;
}
