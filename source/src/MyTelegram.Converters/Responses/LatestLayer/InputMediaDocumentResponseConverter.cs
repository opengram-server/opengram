namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class InputMediaDocumentResponseConverter : IInputMediaDocumentResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IInputMedia ToLayeredData(TInputMediaDocument obj)
    {
        return obj;
    }
}