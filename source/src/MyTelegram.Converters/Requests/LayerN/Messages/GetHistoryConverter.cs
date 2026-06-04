using MyTelegram.Schema.Messages.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Messages;

internal sealed class GetHistoryConverter
    : IRequestConverter<
        RequestGetHistory,
        Schema.Messages.RequestGetHistory
    >, ITransientDependency
{
    public Schema.Messages.RequestGetHistory ToLatestLayerData(IRequestInput request, RequestGetHistory obj)
    {
        return new Schema.Messages.RequestGetHistory
        {
            Peer = obj.Peer,
            OffsetId = obj.OffsetId,
            OffsetDate = obj.OffsetDate,
            AddOffset = obj.AddOffset,
            Limit = obj.Limit,
            MaxId = obj.MaxId,
            MinId = obj.MinId
        };
    }
}