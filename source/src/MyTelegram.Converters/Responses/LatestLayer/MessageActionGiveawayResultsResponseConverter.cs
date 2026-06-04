namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageActionGiveawayResultsResponseConverter : IMessageActionGiveawayResultsResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IMessageAction ToLayeredData(TMessageActionGiveawayResults obj)
    {
        return obj;
    }
}