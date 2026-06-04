namespace MyTelegram.ReadModel.Interfaces;

public interface IDocumentReadModel : IReadModel
{
    long AccessHash { get; }
    byte[]? Attributes { get; }
    long? CreatorId { get; }
    int Date { get; }
    int DcId { get; }
    long DocumentId { get; }
    ReadOnlyMemory<byte> FileReference { get; }
    int? Fingerprint { get; }
    string? Md5CheckSum { get; }
    string? Name { get; }
    string MimeType { get; }
    long Size { get; }

    //byte[]? Stickers { get; }
    long? ThumbId { get; }
    List<PhotoSize>? Thumbs { get; }
    long? VideoThumbId { get; }
    List<VideoSize>? VideoThumbs { get; }
    List<IDocumentAttribute>? Attributes2 { get; }
}