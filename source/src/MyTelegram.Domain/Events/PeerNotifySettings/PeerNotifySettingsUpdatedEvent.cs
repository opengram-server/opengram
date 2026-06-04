namespace MyTelegram.Domain.Events.PeerNotifySettings;

public class PeerNotifySettingsUpdatedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    PeerType peerType,
    long peerId,
    MyTelegram.PeerNotifySettings peerNotifySettings)
    : RequestAggregateEvent2<PeerNotifySettingsAggregate, PeerNotifySettingsId>(requestInfo)
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public long PeerId { get; } = peerId;

    public MyTelegram.PeerNotifySettings PeerNotifySettings { get; } = peerNotifySettings;

    public PeerType PeerType { get; } = peerType;
}
