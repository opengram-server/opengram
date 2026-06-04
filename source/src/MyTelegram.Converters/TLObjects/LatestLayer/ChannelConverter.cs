namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class ChannelConverter(IObjectMapper objectMapper) : IChannelConverter, ITransientDependency
{
    
    public int Layer => Layers.LayerLatest;

    public ILayeredChannel ToChannel(IChannelReadModel channelReadModel)
    {
        return objectMapper.Map<IChannelReadModel, TChannel>(channelReadModel);
    }
}