namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class ChatBannedRightsConverter(IObjectMapper objectMapper)
    : IChatBannedRightsConverter, ITransientDependency
{
    
    public int Layer => Layers.LayerLatest;

    public IChatBannedRights ToChatBannedRights(ChatBannedRights chatBannedRights)
    {
        return objectMapper.Map<ChatBannedRights, TChatBannedRights>(chatBannedRights);
    }
}