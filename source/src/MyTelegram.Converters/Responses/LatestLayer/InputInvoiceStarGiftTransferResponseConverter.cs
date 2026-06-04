namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class InputInvoiceStarGiftTransferResponseConverter : IInputInvoiceStarGiftTransferResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IInputInvoice ToLayeredData(TInputInvoiceStarGiftTransfer obj)
    {
        return obj;
    }
}