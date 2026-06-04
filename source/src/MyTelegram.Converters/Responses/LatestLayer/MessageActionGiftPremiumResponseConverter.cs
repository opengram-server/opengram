namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageActionGiftPremiumResponseConverter : IMessageActionGiftPremiumResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IMessageAction ToLayeredData(TMessageActionGiftPremium obj)
    {
        return obj;
    }
}