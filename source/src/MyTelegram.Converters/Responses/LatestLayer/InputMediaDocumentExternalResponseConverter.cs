namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class InputMediaDocumentExternalResponseConverter : IInputMediaDocumentExternalResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IInputMedia ToLayeredData(TInputMediaDocumentExternal obj)
    {
        return obj;
    }
}