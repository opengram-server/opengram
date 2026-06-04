namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class MessageServiceConverter(IObjectMapper objectMapper)
    : IMessageServiceConverter, ITransientDependency
{
    
    public int Layer => Layers.LayerLatest;

    public ILayeredServiceMessage ToMessage(MessageItem messageItem)
    {
        return objectMapper.Map<MessageItem, TMessageService>(messageItem);
    }

    public ILayeredServiceMessage ToMessage(IMessageReadModel messageReadModel)
    {
        return objectMapper.Map<IMessageReadModel, TMessageService>(messageReadModel);
    }
}