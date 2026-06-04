// ReSharper disable All

namespace MyTelegram.Converters.Responses;

internal sealed class MediaAreaCoordinatesResponseConverter : IMediaAreaCoordinatesResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public Schema.IMediaAreaCoordinates ToLayeredData(Schema.TMediaAreaCoordinates obj)
    {
        return obj;
    }
}