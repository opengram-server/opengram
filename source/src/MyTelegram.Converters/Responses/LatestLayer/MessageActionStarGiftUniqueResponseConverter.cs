namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageActionStarGiftUniqueResponseConverter : IMessageActionStarGiftUniqueResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IMessageAction ToLayeredData(TMessageActionStarGiftUnique obj)
    {
        return obj;
    }
}