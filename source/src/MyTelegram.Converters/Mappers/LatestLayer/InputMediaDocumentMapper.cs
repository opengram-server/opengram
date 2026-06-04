namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class InputMediaDocumentMapper
    : IObjectMapper<TInputMediaDocument, TInputMediaDocument>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public TInputMediaDocument Map(TInputMediaDocument source)
    {
        return Map(source, new TInputMediaDocument());
    }

    public TInputMediaDocument Map(
        TInputMediaDocument source,
        TInputMediaDocument destination
    )
    {
        destination.Spoiler = source.Spoiler;
        destination.Id = source.Id;
        destination.VideoCover = source.VideoCover;
        destination.VideoTimestamp = source.VideoTimestamp;
        destination.TtlSeconds = source.TtlSeconds;
        destination.Query = source.Query;

        return destination;
    }
}