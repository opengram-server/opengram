namespace MyTelegram.Converters.Responses;

public interface IMessageMediaResponseService
{
    IMessageMedia? ToLayeredData(IMessageMedia? latestLayerData, int layer);
    IInputMedia? ToLayeredData(IInputMedia? latestLayerData, int layer);
}