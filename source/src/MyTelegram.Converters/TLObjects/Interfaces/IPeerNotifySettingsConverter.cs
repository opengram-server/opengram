namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IPeerNotifySettingsConverter : ILayeredConverter
{
    IPeerNotifySettings ToPeerNotifySettings(PeerNotifySettings peerNotifySettings);
    IPeerNotifySettings ToPeerNotifySettings(IPeerNotifySettingsReadModel? readModel);
}