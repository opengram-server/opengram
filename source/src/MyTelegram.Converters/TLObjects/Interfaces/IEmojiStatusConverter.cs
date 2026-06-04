namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IEmojiStatusConverter : ILayeredConverter
{
    IEmojiStatus? ToEmojiStatus(EmojiStatus? emojiStatus);
}