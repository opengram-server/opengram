namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class ChannelFullConverter(IObjectMapper objectMapper) : IChannelFullConverter, ITransientDependency
{

    public int Layer => Layers.LayerLatest;

    public ILayeredChannelFull ToChannelFull(IChannelFullReadModel channelFullReadModel)
    {
        return objectMapper.Map<IChannelFullReadModel, TChannelFull>(channelFullReadModel);
    }
}