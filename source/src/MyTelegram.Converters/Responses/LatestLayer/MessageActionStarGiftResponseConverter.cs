namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageActionStarGiftResponseConverter : IMessageActionStarGiftResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IMessageAction ToLayeredData(TMessageActionStarGift obj)
    {
        return obj;
    }
}