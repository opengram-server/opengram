namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageMediaGiveawayResponseConverter : IMessageMediaGiveawayResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IMessageMedia ToLayeredData(TMessageMediaGiveaway obj)
    {
        return obj;
    }
}