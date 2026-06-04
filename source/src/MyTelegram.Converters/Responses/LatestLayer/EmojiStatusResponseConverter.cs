namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class EmojiStatusResponseConverter : IEmojiStatusResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IEmojiStatus? ToLayeredData(TEmojiStatus? obj)
    {
        return obj;
    }
}