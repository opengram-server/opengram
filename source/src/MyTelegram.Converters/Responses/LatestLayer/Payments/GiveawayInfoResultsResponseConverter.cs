using MyTelegram.Schema.Payments;

namespace MyTelegram.Converters.Responses.LatestLayer.Payments;

internal sealed class GiveawayInfoResultsResponseConverter : IGiveawayInfoResultsResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IGiveawayInfo ToLayeredData(TGiveawayInfoResults obj)
    {
        return obj;
    }
}