using MyTelegram.Schema.Channels.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Channels;

internal sealed class GetParticipantConverter
    : IRequestConverter<
        RequestGetParticipant,
        Schema.Channels.RequestGetParticipant
    >, ITransientDependency
{
    public Schema.Channels.RequestGetParticipant ToLatestLayerData(IRequestInput request, RequestGetParticipant obj)
    {
        return new Schema.Channels.RequestGetParticipant
        {
            Channel = obj.Channel,
            Participant = obj.UserId.ToInputPeer()
        };
    }
}