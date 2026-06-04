namespace MyTelegram.Services.TLObjectConverters;

public interface IRequestConverter
{
}

public interface IRequestConverter<in TOldLayerRequestData, out TNewLayerRequestData> : IRequestConverter
{
    TNewLayerRequestData ToLatestLayerData(IRequestInput request, TOldLayerRequestData data);
}