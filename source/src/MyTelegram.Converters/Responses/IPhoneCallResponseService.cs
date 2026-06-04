namespace MyTelegram.Converters.Responses;

public interface IPhoneCallResponseService
{
    IPhoneCall ToLayeredData(IPhoneCall latestLayerData, int layer);
}