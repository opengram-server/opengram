namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class InputInvoiceStarsResponseConverter : IInputInvoiceStarsResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IInputInvoice ToLayeredData(TInputInvoiceStars obj)
    {
        return obj;
    }
}