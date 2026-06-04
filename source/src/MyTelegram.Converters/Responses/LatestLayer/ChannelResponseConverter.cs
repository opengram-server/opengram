namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class ChannelResponseConverter : IChannelResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public ILayeredChannel ToLayeredData(TChannel obj)
    {
        return obj;
    }
}