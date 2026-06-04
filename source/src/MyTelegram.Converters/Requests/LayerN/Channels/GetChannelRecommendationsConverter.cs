using MyTelegram.Schema.Channels.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Channels;

internal sealed class GetChannelRecommendationsConverter
    : IRequestConverter<
        RequestGetChannelRecommendations,
        Schema.Channels.RequestGetChannelRecommendations
    >, ITransientDependency
{
    public Schema.Channels.RequestGetChannelRecommendations ToLatestLayerData(IRequestInput request,
        RequestGetChannelRecommendations obj)
    {
        return new Schema.Channels.RequestGetChannelRecommendations
        {
            Channel = obj.Channel
        };
    }
}