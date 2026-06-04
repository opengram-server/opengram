namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class InputMediaUploadedDocumentMapper
    : IObjectMapper<TInputMediaUploadedDocument, TInputMediaUploadedDocument>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public TInputMediaUploadedDocument Map(TInputMediaUploadedDocument source)
    {
        return Map(source, new TInputMediaUploadedDocument());
    }

    public TInputMediaUploadedDocument Map(
        TInputMediaUploadedDocument source,
        TInputMediaUploadedDocument destination
    )
    {
        destination.NosoundVideo = source.NosoundVideo;
        destination.ForceFile = source.ForceFile;
        destination.Spoiler = source.Spoiler;
        destination.File = source.File;
        destination.Thumb = source.Thumb;
        destination.MimeType = source.MimeType;
        destination.Attributes = source.Attributes;
        destination.Stickers = source.Stickers;
        destination.VideoCover = source.VideoCover;
        destination.VideoTimestamp = source.VideoTimestamp;
        destination.TtlSeconds = source.TtlSeconds;

        return destination;
    }
}