namespace MyTelegram.ReadModel.Interfaces;

public interface IPhotoReadModel : IReadModel
{
    string Id { get; }
    long AccessHash { get; }
    int Date { get; }
    int DcId { get; }
    byte[] FileReference { get; }
    bool HasStickers { get; }
    bool HasVideo { get; }
    long PhotoId { get; }
    long Size { get; }
    List<PhotoSize>? Sizes { get; }
    long UserId { get; }
    List<VideoSize>? VideoSizes { get; }
    bool IsProfilePhoto { get; }

    List<IPhotoSize>? Sizes2 { get; }
    List<IVideoSize>? VideoSizes2 { get; }
}