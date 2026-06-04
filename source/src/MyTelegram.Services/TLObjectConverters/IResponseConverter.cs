namespace MyTelegram.Services.TLObjectConverters;

public interface IResponseConverter : ILayeredConverter
{
}

public interface IResponseConverter<in TLatestResponseData, out TOldLayerResponseData> : IResponseConverter
{
    TOldLayerResponseData ToLayeredData(TLatestResponseData data);
    //TOldLayerResponseData ToLatestLayerData(TOldLayerResponseData oldLayerData);
}