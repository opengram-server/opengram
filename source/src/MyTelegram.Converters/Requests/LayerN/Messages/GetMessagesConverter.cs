using MyTelegram.Schema.Messages.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Messages;

internal sealed class GetMessagesConverter
    : IRequestConverter<
        RequestGetMessages,
        Schema.Messages.RequestGetMessages
    >, ITransientDependency
{
    public Schema.Messages.RequestGetMessages ToLatestLayerData(IRequestInput request, RequestGetMessages obj)
    {
        var ids = obj.Id.Select(p => (IInputMessage)new TInputMessageID { Id = p });
        return new Schema.Messages.RequestGetMessages
        {
            Id = new TVector<IInputMessage>(ids)
        };
    }
}