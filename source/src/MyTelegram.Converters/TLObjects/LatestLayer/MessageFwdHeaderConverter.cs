namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class MessageFwdHeaderConverter(IObjectMapper objectMapper)
    : IMessageFwdHeaderConverter, ITransientDependency
{
    
    public int Layer => Layers.LayerLatest;

    public IMessageFwdHeader? ToMessageFwdHeader(MessageFwdHeader? fwdHeader)
    {
        if (fwdHeader == null)
        {
            return null;
        }

        return objectMapper.Map<MessageFwdHeader, TMessageFwdHeader>(fwdHeader);
    }
}