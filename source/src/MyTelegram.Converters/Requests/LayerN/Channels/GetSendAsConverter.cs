using MyTelegram.Schema.Channels.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Channels;

internal sealed class GetSendAsConverter
    : IRequestConverter<
        RequestGetSendAs,
        Schema.Channels.RequestGetSendAs
    >, ITransientDependency
{
    public Schema.Channels.RequestGetSendAs ToLatestLayerData(IRequestInput request, RequestGetSendAs obj)
    {
        return new Schema.Channels.RequestGetSendAs
        {
            Peer = obj.Peer
        };
    }
}