namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class StarGiftUniqueResponseConverter : IStarGiftUniqueResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IStarGift ToLayeredData(TStarGiftUnique obj)
    {
        return obj;
    }
}