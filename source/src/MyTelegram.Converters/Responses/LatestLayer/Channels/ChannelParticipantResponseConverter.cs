using IChannelParticipant = MyTelegram.Schema.Channels.IChannelParticipant;
using IChannelParticipantResponseConverter =
    MyTelegram.Converters.Responses.Interfaces.Channels.IChannelParticipantResponseConverter;
using TChannelParticipant = MyTelegram.Schema.Channels.TChannelParticipant;

namespace MyTelegram.Converters.Responses.LatestLayer.Channels;

internal sealed class ChannelParticipantResponseConverter : IChannelParticipantResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChannelParticipant ToLayeredData(TChannelParticipant obj)
    {
        return obj;
    }
}