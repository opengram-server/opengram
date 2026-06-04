namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class ChannelParticipantCreatorResponseConverter : IChannelParticipantCreatorResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChannelParticipant ToLayeredData(TChannelParticipantCreator obj)
    {
        return obj;
    }
}