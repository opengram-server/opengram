namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class ChannelParticipantSelfResponseConverter : IChannelParticipantSelfResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChannelParticipant ToLayeredData(TChannelParticipantSelf obj)
    {
        return obj;
    }
}