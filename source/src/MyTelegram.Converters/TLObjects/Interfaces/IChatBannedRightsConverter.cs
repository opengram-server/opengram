namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IChatBannedRightsConverter : ILayeredConverter
{
    IChatBannedRights ToChatBannedRights(ChatBannedRights chatBannedRights);
}