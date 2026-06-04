namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class InputInvoiceStarGiftResponseConverter : IInputInvoiceStarGiftResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IInputInvoice ToLayeredData(TInputInvoiceStarGift obj)
    {
        return obj;
    }
}