namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class WebViewResultUrlResponseConverter : IWebViewResultUrlResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IWebViewResult ToLayeredData(TWebViewResultUrl obj)
    {
        return obj;
    }
}