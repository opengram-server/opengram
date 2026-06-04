namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class ChannelParticipantAdminResponseConverter : IChannelParticipantAdminResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChannelParticipant ToLayeredData(TChannelParticipantAdmin obj)
    {
        return obj;
    }
}