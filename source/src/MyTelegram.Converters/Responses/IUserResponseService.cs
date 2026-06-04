namespace MyTelegram.Converters.Responses;

public interface IUserResponseService
{
    ILayeredUser ToLayeredData(TUser latestLayerData, int layer);
}