namespace MyTelegram.Domain.Aggregates.PeerSetting;

public class PeerSettingsSnapshot(
    PeerSettings peerSettings,
    bool hidePeerSettingsBar,
    long ownerPeerId,
    long peerId)
    : ISnapshot
{
    public PeerSettings PeerSettings { get; } = peerSettings;
    public bool HidePeerSettingsBar { get; private set; } = hidePeerSettingsBar;
    public long OwnerPeerId { get; private set; } = ownerPeerId;
    public long PeerId { get; private set; } = peerId;
}