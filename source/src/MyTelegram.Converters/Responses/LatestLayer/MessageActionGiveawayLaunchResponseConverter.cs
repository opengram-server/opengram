namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageActionGiveawayLaunchResponseConverter : IMessageActionGiveawayLaunchResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IMessageAction ToLayeredData(TMessageActionGiveawayLaunch obj)
    {
        return obj;
    }
}