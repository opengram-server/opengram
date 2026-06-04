namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IPeerSettingsConverter : ILayeredConverter
{
    IPeerSettings ToPeerSettings(long selfUserId, long targetUserId, IPeerSettingsReadModel? readModel,
        ContactType? contactType);
}