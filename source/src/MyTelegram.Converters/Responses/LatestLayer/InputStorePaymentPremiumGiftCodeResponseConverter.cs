namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class
    InputStorePaymentPremiumGiftCodeResponseConverter : IInputStorePaymentPremiumGiftCodeResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IInputStorePaymentPurpose ToLayeredData(TInputStorePaymentPremiumGiftCode obj)
    {
        return obj;
    }
}