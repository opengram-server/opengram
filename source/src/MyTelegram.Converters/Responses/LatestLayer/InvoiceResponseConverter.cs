namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class InvoiceResponseConverter : IInvoiceResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IInvoice ToLayeredData(TInvoice obj)
    {
        return obj;
    }
}