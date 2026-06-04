namespace MyTelegram;

public record ThemeSettings(
    bool MessageColorsAnimated,
    MyTelegram.Schema.IBaseTheme BaseTheme,
    int AccentColor,
    int? OutboxAccentColor,
    List<int>? MessageColors,
    //long? WallPaperId,
    WallPaper? WallPaper);