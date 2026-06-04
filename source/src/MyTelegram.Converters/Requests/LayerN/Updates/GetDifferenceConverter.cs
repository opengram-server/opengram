using MyTelegram.Schema.Updates.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Updates;

internal sealed class GetDifferenceConverter
    : IRequestConverter<
        RequestGetDifference,
        Schema.Updates.RequestGetDifference
    >, ITransientDependency
{
    public Schema.Updates.RequestGetDifference ToLatestLayerData(IRequestInput request, RequestGetDifference obj)
    {
        return new Schema.Updates.RequestGetDifference
        {
            Pts = obj.Pts,
            PtsTotalLimit = obj.PtsTotalLimit,
            Date = obj.Date,
            Qts = obj.Qts
        };
    }
}