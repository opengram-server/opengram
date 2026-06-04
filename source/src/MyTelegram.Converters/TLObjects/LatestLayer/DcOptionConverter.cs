namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class DcOptionConverter : IDcOptionConverter, ITransientDependency
{
    
    public int Layer => Layers.LayerLatest;
}