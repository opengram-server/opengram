namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class ChatAdminRightsConverter(IObjectMapper objectMapper)
    : IChatAdminRightsConverter, ITransientDependency
{
    
    public int Layer => Layers.LayerLatest;

    public IChatAdminRights ToChatAdminRights(ChatAdminRights chatAdminRights)
    {
        return objectMapper.Map<ChatAdminRights, TChatAdminRights>(chatAdminRights);
    }
}