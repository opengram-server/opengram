namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class InputMediaUploadedDocumentResponseConverter : IInputMediaUploadedDocumentResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IInputMedia ToLayeredData(TInputMediaUploadedDocument obj)
    {
        return obj;
    }

    public IInputMedia ToLatestLayerData(IInputMedia oldLayerData)
    {
        return oldLayerData;
    }
}