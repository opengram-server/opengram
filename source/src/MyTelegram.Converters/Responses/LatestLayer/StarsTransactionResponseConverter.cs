namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class StarsTransactionResponseConverter : IStarsTransactionResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IStarsTransaction ToLayeredData(TStarsTransaction obj)
    {
        return obj;
    }
}