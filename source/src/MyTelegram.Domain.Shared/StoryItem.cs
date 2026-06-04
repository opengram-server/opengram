namespace MyTelegram;

public record StoryItem(
    int Id,
    Peer Peer,
    IMessageMedia Media,
    long RandomId,
    List<PrivacyValueData> PrivacyRules,
    int Date,
    int ExpireDate,
    Peer? FromPeer = null,
    string? Caption = null,
    List<IMediaArea>? MediaAreas = null,
    bool Pinned = false,
    bool NoForwards = false,
    List<IMessageEntity>? Entities = null,
    int? Period = null,
    Peer? FwdFromId = null,
    int? FwdFromStory = null
);