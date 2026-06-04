namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class MessageConverter(IObjectMapper objectMapper) : IMessageConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public ILayeredMessage ToMessage(IMessageReadModel messageReadModel)
    {
        return objectMapper.Map<IMessageReadModel, TMessage>(messageReadModel);
    }

    public ILayeredMessage ToMessage(MessageItem messageItem)
    {
        return objectMapper.Map<MessageItem, TMessage>(messageItem);
    }
}