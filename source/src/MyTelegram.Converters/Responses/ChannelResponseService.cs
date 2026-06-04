namespace MyTelegram.Converters.Responses;

public class ChannelResponseService(
    ILayeredService<IChannelResponseConverter> channelResponseLayeredService,
    IEmojiStatusResponseService emojiStatusResponseService) : IChannelResponseService, ITransientDependency
{
    public ILayeredChannel ToLayeredData(TChannel latestLayerData, int layer)
    {
        var targetLayerData = channelResponseLayeredService.GetConverter(layer).ToLayeredData(latestLayerData);
        targetLayerData.EmojiStatus = emojiStatusResponseService.ToLayeredData(targetLayerData.EmojiStatus, layer);

        return targetLayerData;
    }
}