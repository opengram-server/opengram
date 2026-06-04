namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class InputMediaDocumentExternalMapper
    : IObjectMapper<TInputMediaDocumentExternal, TInputMediaDocumentExternal>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public TInputMediaDocumentExternal Map(TInputMediaDocumentExternal source)
    {
        return Map(source, new TInputMediaDocumentExternal());
    }

    public TInputMediaDocumentExternal Map(
        TInputMediaDocumentExternal source,
        TInputMediaDocumentExternal destination
    )
    {
        destination.Spoiler = source.Spoiler;
        destination.Url = source.Url;
        destination.TtlSeconds = source.TtlSeconds;
        destination.VideoCover = source.VideoCover;
        destination.VideoTimestamp = source.VideoTimestamp;

        return destination;
    }
}