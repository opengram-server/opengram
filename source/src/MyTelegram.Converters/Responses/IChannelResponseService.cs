namespace MyTelegram.Converters.Responses;

public interface IChannelResponseService
{
    ILayeredChannel ToLayeredData(TChannel latestLayerData, int layer);
}