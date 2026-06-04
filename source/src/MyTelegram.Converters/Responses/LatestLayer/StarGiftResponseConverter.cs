namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class StarGiftResponseConverter : IStarGiftResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IStarGift ToLayeredData(TStarGift obj)
    {
        return obj;
    }
}