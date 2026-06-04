namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class ChannelAdminLogEventsFilterResponseConverter : IChannelAdminLogEventsFilterResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChannelAdminLogEventsFilter ToLayeredData(TChannelAdminLogEventsFilter obj)
    {
        return obj;
    }
}