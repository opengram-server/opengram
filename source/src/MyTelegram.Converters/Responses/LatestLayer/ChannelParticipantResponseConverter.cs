using IChannelParticipantResponseConverter =
    MyTelegram.Converters.Responses.Interfaces.IChannelParticipantResponseConverter;

namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class ChannelParticipantResponseConverter : IChannelParticipantResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChannelParticipant ToLayeredData(TChannelParticipant obj)
    {
        return obj;
    }
}