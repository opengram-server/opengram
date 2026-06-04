namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class MessageMediaDocumentResponseConverter : IMessageMediaDocumentResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IMessageMedia ToLayeredData(TMessageMediaDocument obj)
    {
        return new Schema.TMessageMediaDocument
        {
            Nopremium = obj.Nopremium,
            Spoiler = obj.Spoiler,
            Video = obj.Video,
            Round = obj.Round,
            Voice = obj.Voice,
            Document = obj.Document,
            AltDocuments = obj.AltDocuments,
            TtlSeconds = obj.TtlSeconds,
            VideoCover=obj.VideoCover,
            VideoTimestamp = obj.VideoTimestamp,
        };
    }
}