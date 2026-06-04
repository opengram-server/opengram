namespace MyTelegram.Converters.Responses;

public class EmojiStatusResponseService(ILayeredService<IEmojiStatusResponseConverter> emojiStatusLayeredService)
    : IEmojiStatusResponseService, ITransientDependency
{
    public IEmojiStatus? ToLayeredData(IEmojiStatus? latestLayerData, int layer)
    {
        switch (latestLayerData)
        {
            case TEmojiStatus emojiStatus:
                return emojiStatusLayeredService.GetConverter(layer).ToLayeredData(emojiStatus);
        }

        return latestLayerData;
    }
}