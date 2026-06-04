namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageActionGiftCodeResponseConverter : IMessageActionGiftCodeResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IMessageAction ToLayeredData(TMessageActionGiftCode obj)
    {
        return obj;
    }
}