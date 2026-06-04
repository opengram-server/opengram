namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class
    StarGiftAttributeOriginalDetailsResponseConverter : IStarGiftAttributeOriginalDetailsResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IStarGiftAttribute ToLayeredData(TStarGiftAttributeOriginalDetails obj)
    {
        return obj;
    }
}