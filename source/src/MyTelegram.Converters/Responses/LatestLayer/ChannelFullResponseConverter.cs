namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class ChannelFullResponseConverter : IChannelFullResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChatFull ToLayeredData(TChannelFull obj)
    {
        return obj;
    }
}