namespace MyTelegram;

public record Theme(
    bool Default,
    bool ForChat,
    long Id,
    long AccessHash,
    string Slug,
    string Title,
    long? DocumentId,
    List<ThemeSettings> Settings,
    string? Emoticon,
    string Format);