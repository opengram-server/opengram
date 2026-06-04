namespace MyTelegram.ReadModel.Interfaces;

public interface IStoryReadModel : IReadModel
{
    long OwnerPeerId { get; }
    int StoryId { get; }
    IMessageMedia Media { get; }
    long RandomId { get; }
    List<PrivacyValueData> PrivacyRules { get; }
    int Date { get; }
    int ExpireDate { get; }
    Peer? FromPeer { get; }
    string? Caption { get; }
    List<IMediaArea>? MediaAreas { get; }
    bool Pinned { get; }
    bool NoForwards { get; }
    List<IMessageEntity>? Entities { get; }
    int? Period { get; }
    Peer? FwdFromId { get; }
    int? FwdFromStory { get; }
    bool Archived { get; }
    int ViewsCount { get; }
    bool IsDeleted { get; }
}