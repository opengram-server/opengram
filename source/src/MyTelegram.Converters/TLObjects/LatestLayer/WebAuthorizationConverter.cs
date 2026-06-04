namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class WebAuthorizationConverter : IWebAuthorizationConverter, ITransientDependency
{
    
    public int Layer => Layers.LayerLatest;
}