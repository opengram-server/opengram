namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class StarsSubscriptionResponseConverter : IStarsSubscriptionResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IStarsSubscription ToLayeredData(TStarsSubscription obj)
    {
        return obj;
    }
}