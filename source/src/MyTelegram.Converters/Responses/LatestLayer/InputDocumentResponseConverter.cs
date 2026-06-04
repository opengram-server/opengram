namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class InputDocumentResponseConverter : IInputDocumentResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IInputDocument ToLayeredData(TInputDocument obj)
    {
        return obj;
    }
}