namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class InputInvoiceStarGiftUpgradeResponseConverter : IInputInvoiceStarGiftUpgradeResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IInputInvoice ToLayeredData(TInputInvoiceStarGiftUpgrade obj)
    {
        return obj;
    }
}