using MyTelegram.Schema.Payments;

namespace MyTelegram.Converters.Responses.LatestLayer.Payments;

internal sealed class StarsStatusResponseConverter : IStarsStatusResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IStarsStatus ToLayeredData(TStarsStatus obj)
    {
        return obj;
    }
}