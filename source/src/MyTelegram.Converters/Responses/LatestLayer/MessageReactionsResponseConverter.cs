namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageReactionsResponseConverter : IMessageReactionsResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IMessageReactions ToLayeredData(TMessageReactions obj)
    {
        return obj;
    }
}