namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageResponseConverter : IMessageResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public ILayeredMessage ToLayeredData(TMessage obj)
    {
        return obj;
    }
}