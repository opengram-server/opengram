namespace MyTelegram;

public record WallPaperSettings(
    bool Blur,
    bool Motion,
    int? BackgroundColor,
    int? SecondBackgroundColor,
    int? ThirdBackgroundColor,
    int? FourthBackgroundColor,
    int? Intensity,
    int? Rotation,
    string? Emoticon
);