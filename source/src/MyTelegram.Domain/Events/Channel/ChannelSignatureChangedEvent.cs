namespace MyTelegram.Domain.Events.Channel;

public class ChannelSignatureChangedEvent(RequestInfo requestInfo, long channelId, bool signatureEnabled, bool profilesEnabled)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public bool SignatureEnabled { get; } = signatureEnabled;
    public bool ProfilesEnabled { get; } = profilesEnabled;
}