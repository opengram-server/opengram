namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class BotInfoResponseConverter : IBotInfoResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IBotInfo ToLayeredData(TBotInfo obj)
    {
        return obj;
    }
}