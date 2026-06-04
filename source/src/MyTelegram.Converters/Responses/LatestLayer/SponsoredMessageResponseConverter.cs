namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class SponsoredMessageResponseConverter : ISponsoredMessageResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public ISponsoredMessage ToLayeredData(TSponsoredMessage obj)
    {
        return obj;
    }
}