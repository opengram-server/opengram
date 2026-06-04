namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class BoostResponseConverter : IBoostResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IBoost ToLayeredData(TBoost obj)
    {
        return obj;
    }
}