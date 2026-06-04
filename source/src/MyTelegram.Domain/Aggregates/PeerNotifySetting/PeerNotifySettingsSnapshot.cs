namespace MyTelegram.Domain.Aggregates.PeerNotifySetting;

public class PeerNotifySettingsSnapshot(MyTelegram.PeerNotifySettings peerNotifySettings) : ISnapshot
{
    public MyTelegram.PeerNotifySettings PeerNotifySettings { get; } = peerNotifySettings;
}
