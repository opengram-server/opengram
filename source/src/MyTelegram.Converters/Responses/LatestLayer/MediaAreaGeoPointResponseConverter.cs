// ReSharper disable All

namespace MyTelegram.Converters.Responses;

internal sealed class MediaAreaGeoPointResponseConverter : IMediaAreaGeoPointResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public Schema.IMediaArea ToLayeredData(Schema.TMediaAreaGeoPoint obj)
    {
        return obj;
    }
}