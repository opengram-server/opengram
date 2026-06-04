namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class UpdateStarsBalanceResponseConverter : IUpdateStarsBalanceResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IUpdate ToLayeredData(TUpdateStarsBalance obj)
    {
        return obj;
    }
}