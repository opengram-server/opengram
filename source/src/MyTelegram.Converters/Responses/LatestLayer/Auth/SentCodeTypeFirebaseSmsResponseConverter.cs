using MyTelegram.Schema.Auth;

namespace MyTelegram.Converters.Responses.LatestLayer.Auth;

internal sealed class SentCodeTypeFirebaseSmsResponseConverter : ISentCodeTypeFirebaseSmsResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public ISentCodeType ToLayeredData(TSentCodeTypeFirebaseSms obj)
    {
        return obj;
    }
}