namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IThemeConverter : ILayeredConverter
{
    ITheme ToTheme(Theme theTheme, IReadOnlyCollection<IDocumentReadModel> documentReadModels);

    ITheme ToTheme(long selfUserId, IThemeReadModel themeReadModel,
        IReadOnlyCollection<IDocumentReadModel> documentReadModels);
}

//public interface IPeerSettingsConverter : ILayeredConverter
//{
//    IPeerSettings ToPeerSettings(long selfUserId, long targetUserId, IPeerSettingsReadModel? readModel,
//        ContactType? contactType);
//}
//public interface IPeerNotifySettingsConverter : ILayeredConverter
//{
//    IPeerNotifySettings ToPeerNotifySettings(IPeerNotifySettingsReadModel? readModel);
//    IPeerNotifySettings ToPeerNotifySettings(PeerNotifySettings? peerNotifySettings);
//}