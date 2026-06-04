namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageMediaGiveawayResultsResponseConverter : IMessageMediaGiveawayResultsResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IMessageMedia ToLayeredData(TMessageMediaGiveawayResults obj)
    {
        return obj;
    }
}