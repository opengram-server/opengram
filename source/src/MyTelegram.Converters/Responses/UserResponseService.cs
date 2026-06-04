namespace MyTelegram.Converters.Responses;

public class UserResponseService(
    ILayeredService<IUserResponseConverter> userResponseLayeredService,
    IEmojiStatusResponseService emojiStatusResponseService) : IUserResponseService, ITransientDependency
{
    public ILayeredUser ToLayeredData(TUser latestLayerData, int layer)
    {
        var targetLayerData = userResponseLayeredService.GetConverter(layer).ToLayeredData(latestLayerData);
        targetLayerData.EmojiStatus =
            emojiStatusResponseService.ToLayeredData(targetLayerData.EmojiStatus, layer);

        return targetLayerData;
    }
}