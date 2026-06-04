namespace MyTelegram;

public record DialogFilter
(
    int Id,
    bool Contacts,
    bool NonContacts,
    bool Groups,
    bool Broadcasts,
    bool Bots,
    bool ExcludeMuted,
    bool ExcludeRead,
    bool ExcludeArchived,
    bool TitleNoAnimate,
    ITextWithEntities Title,
    string? Emoticon,
    int? Color,
    IList<InputPeer> PinnedPeers,
    IList<InputPeer> IncludePeers,
    IList<InputPeer> ExcludePeers,
    bool IsChatlist
);