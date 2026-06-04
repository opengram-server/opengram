using MyTelegram.Schema.Channels.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Channels;

internal sealed class GetMessagesConverter
    : IRequestConverter<
        RequestGetMessages,
        Schema.Channels.RequestGetMessages
    >, ITransientDependency
{
    public Schema.Channels.RequestGetMessages ToLatestLayerData(IRequestInput request, RequestGetMessages obj)
    {
        var ids = obj.Id.Select(p => (IInputMessage)new TInputMessageID { Id = p });
        return new Schema.Channels.RequestGetMessages
        {
            Channel = obj.Channel,
            Id = new TVector<IInputMessage>(ids)
        };
    }
}