using MyTelegram.Schema.Channels;

namespace MyTelegram.Converters.Responses.LatestLayer.Channels;

internal sealed class ChannelParticipantsResponseConverter : IChannelParticipantsResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChannelParticipants ToLayeredData(TChannelParticipants obj)
    {
        return obj;
    }
}