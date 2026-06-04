namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class ChannelParticipantBannedResponseConverter : IChannelParticipantBannedResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChannelParticipant ToLayeredData(TChannelParticipantBanned obj)
    {
        return obj;
    }
}