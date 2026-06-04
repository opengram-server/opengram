using MyTelegram.Schema.Auth;

namespace MyTelegram.Converters.Responses.LatestLayer.Auth;

internal sealed class SentCodeResponseConverter : ISentCodeResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public ISentCode ToLayeredData(TSentCode obj)
    {
        return obj;
    }
}