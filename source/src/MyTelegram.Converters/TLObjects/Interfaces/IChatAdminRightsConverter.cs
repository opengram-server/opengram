namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IChatAdminRightsConverter : ILayeredConverter
{
    IChatAdminRights ToChatAdminRights(ChatAdminRights chatAdminRights);
}