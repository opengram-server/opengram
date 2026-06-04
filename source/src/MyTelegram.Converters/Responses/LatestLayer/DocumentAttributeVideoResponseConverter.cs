namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class DocumentAttributeVideoResponseConverter : IDocumentAttributeVideoResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IDocumentAttribute ToLayeredData(TDocumentAttributeVideo obj)
    {
        return obj;
    }
}