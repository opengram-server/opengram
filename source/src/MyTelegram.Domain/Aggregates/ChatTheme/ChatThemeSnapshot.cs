namespace MyTelegram.Domain.Aggregates.ChatTheme;

public record ChatThemeSnapshot(
    long OwnerPeerId,
    PeerType PeerType,
    long PeerId,
    string? Emoticon,
    int? MessageId,
    int SetDate
) : ISnapshot;
