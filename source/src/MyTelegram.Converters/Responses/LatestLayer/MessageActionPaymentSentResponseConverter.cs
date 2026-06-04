namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageActionPaymentSentResponseConverter : IMessageActionPaymentSentResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IMessageAction ToLayeredData(TMessageActionPaymentSent obj)
    {
        return obj;
    }
}