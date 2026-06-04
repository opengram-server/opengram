namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class ChannelParticipantLeftResponseConverter : IChannelParticipantLeftResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChannelParticipant ToLayeredData(TChannelParticipantLeft obj)
    {
        return obj;
    }
}