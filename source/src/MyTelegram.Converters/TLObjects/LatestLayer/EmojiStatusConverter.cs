namespace MyTelegram.Converters.TLObjects.LatestLayer;

public class EmojiStatusConverter : IEmojiStatusConverter, ITransientDependency
{
    public IEmojiStatus? ToEmojiStatus(EmojiStatus? emojiStatus)
    {
        if (emojiStatus == null)
        {
            return null;
        }

        return new TEmojiStatus
        {
            DocumentId = emojiStatus.DocumentId,
            Until = emojiStatus.Until
        };
    }

    public int Layer => Layers.LayerLatest;
}