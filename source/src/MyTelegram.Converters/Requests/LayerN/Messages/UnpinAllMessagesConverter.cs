using MyTelegram.Schema.Messages.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Messages;

internal sealed class UnpinAllMessagesConverter
    : IRequestConverter<
        RequestUnpinAllMessages,
        Schema.Messages.RequestUnpinAllMessages
    >, ITransientDependency
{
    public Schema.Messages.RequestUnpinAllMessages ToLatestLayerData(IRequestInput request, RequestUnpinAllMessages obj)
    {
        return new Schema.Messages.RequestUnpinAllMessages
        {
            Peer = obj.Peer
        };
    }
}