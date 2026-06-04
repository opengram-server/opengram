namespace MyTelegram.Converters.Responses;

public interface IEmojiStatusResponseService
{
    IEmojiStatus? ToLayeredData(IEmojiStatus? latestLayerData, int layer);
}