namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageServiceResponseConverter : IMessageServiceResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public ILayeredServiceMessage ToLayeredData(TMessageService obj)
    {
        return obj;
    }
}