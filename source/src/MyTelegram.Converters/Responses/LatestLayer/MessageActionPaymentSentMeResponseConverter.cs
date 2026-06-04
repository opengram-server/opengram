namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageActionPaymentSentMeResponseConverter : IMessageActionPaymentSentMeResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IMessageAction ToLayeredData(TMessageActionPaymentSentMe obj)
    {
        return obj;
    }
}