namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class UpdateMessageExtendedMediaResponseConverter : IUpdateMessageExtendedMediaResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IUpdate ToLayeredData(TUpdateMessageExtendedMedia obj)
    {
        return obj;
    }
}