using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MyTelegram.QueryHandlers.MongoDB;

// Строго типизированный класс документа, чтобы гарантировать корректную кодировку UTF-8
public class ChatThemeDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = default!;
    
    [BsonElement("Emoticon")]
    public string Emoticon { get; set; } = default!;
    
    [BsonElement("DocumentId")]
    [BsonIgnoreIfNull]
    public long? DocumentId { get; set; }
}

public class GetAllChatThemesQueryHandler(IMongoDatabase mongoDatabase, ILogger<GetAllChatThemesQueryHandler> logger) : IQueryHandler<GetAllChatThemesQuery, IReadOnlyList<Theme>>
{
    public async Task<IReadOnlyList<Theme>> ExecuteQueryAsync(GetAllChatThemesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            // Читаем темы из MongoDB. Берём строго типизированную коллекцию, чтобы не было проблем с кодировкой.
            var collection = mongoDatabase.GetCollection<ChatThemeDocument>("chat_themes");
            var themeDocs = await collection.Find(Builders<ChatThemeDocument>.Filter.Empty).ToListAsync(cancellationToken);

            logger.LogInformation("GetAllChatThemesQueryHandler: Found {Count} chat themes in database", themeDocs.Count);

            var themes = new List<Theme>();
            foreach (var doc in themeDocs)
            {
                try
                {
                    // Простая тема — только эмодзи
                    var emoticon = doc.Emoticon;
                    
                    if (string.IsNullOrEmpty(emoticon))
                    {
                        logger.LogWarning("Skipping theme without emoticon");
                        continue;
                    }
                    
                    logger.LogInformation("GetAllChatThemesQueryHandler: Processing emoticon='{Emoticon}' (length={Length}, bytes={Bytes})", 
                        emoticon, emoticon.Length, string.Join(",", System.Text.Encoding.UTF8.GetBytes(emoticon)));
                    
                    // Формируем ID из хэша эмодзи
                    var id = (long)emoticon.GetHashCode();
                    var accessHash = id * 31;
                    var slug = $"theme-{emoticon}";
                    var title = emoticon;
                    var isDefault = true;
                    var forChat = true;
                    
                    long? documentId = doc.DocumentId;

                    // У темы чата обязательно должны быть настройки — клиент их проверяет.
                    // Создаём для каждой темы светлый и тёмный варианты.
                    var settings = new List<ThemeSettings>();
                    var (lightColor1, lightColor2) = GetColorsForEmoticon(emoticon, false);
                    var (darkColor1, darkColor2) = GetColorsForEmoticon(emoticon, true);
                    
                    // Светлая тема (день)
                    var lightWallpaperSettings = new WallPaperSettings(
                        Blur: false,
                        Motion: true,
                        BackgroundColor: lightColor1,
                        SecondBackgroundColor: lightColor2,
                        ThirdBackgroundColor: null,
                        FourthBackgroundColor: null,
                        Intensity: 50,
                        Rotation: 45,
                        Emoticon: emoticon
                    );
                    
                    var lightWallpaper = new WallPaper(
                        Id: id + 1000,
                        AccessHash: accessHash + 1000,
                        Default: true,
                        Pattern: false,
                        Dark: false,
                        Slug: $"{slug}-light",
                        DocumentId: null,
                        Settings: lightWallpaperSettings
                    );
                    
                    settings.Add(new ThemeSettings(
                        MessageColorsAnimated: false,
                        BaseTheme: new MyTelegram.Schema.TBaseThemeDay(), // Дневная тема
                        AccentColor: lightColor1,
                        OutboxAccentColor: lightColor2,
                        MessageColors: new List<int> { lightColor1, lightColor2 },
                        WallPaper: lightWallpaper
                    ));
                    
                    // Тёмная тема (ночь)
                    var darkWallpaperSettings = new WallPaperSettings(
                        Blur: false,
                        Motion: true,
                        BackgroundColor: darkColor1,
                        SecondBackgroundColor: darkColor2,
                        ThirdBackgroundColor: null,
                        FourthBackgroundColor: null,
                        Intensity: 50,
                        Rotation: 45,
                        Emoticon: emoticon
                    );
                    
                    var darkWallpaper = new WallPaper(
                        Id: id + 2000,
                        AccessHash: accessHash + 2000,
                        Default: true,
                        Pattern: false,
                        Dark: true,
                        Slug: $"{slug}-dark",
                        DocumentId: null,
                        Settings: darkWallpaperSettings
                    );
                    
                    settings.Add(new ThemeSettings(
                        MessageColorsAnimated: false,
                        BaseTheme: new MyTelegram.Schema.TBaseThemeNight(), // Ночная тема
                        AccentColor: darkColor1,
                        OutboxAccentColor: darkColor2,
                        MessageColors: new List<int> { darkColor1, darkColor2 },
                        WallPaper: darkWallpaper
                    ));

                    themes.Add(new Theme(
                        Default: isDefault,
                        ForChat: forChat,
                        Id: id,
                        AccessHash: accessHash,
                        Slug: slug,
                        Title: title,
                        DocumentId: documentId,
                        Settings: settings,
                        Emoticon: emoticon,
                        Format: "tdesktop"
                    ));
                }
                catch (Exception ex)
                {
                    // Пропускаем некорректную тему
                    logger.LogError(ex, "Error parsing theme");
                }
            }

            logger.LogInformation("GetAllChatThemesQueryHandler: Successfully parsed {Count} themes", themes.Count);
            return themes;
        }
        catch (Exception)
        {
            // При ошибке возвращаем пустой список
            return new List<Theme>();
        }
    }
    
    private static (int color1, int color2) GetColorsForEmoticon(string emoticon, bool isDark)
    {
        // Сопоставляем эмодзи с цветами градиента (RGB-24).
        // Светлые варианты — яркие и насыщенные.
        // Тёмные варианты — глубже и темнее для ночного режима.
        return emoticon switch
        {
            "🏠" => isDark
                ? (0x0a3d5c, 0x1565a0) // Тёмно-синий
                : (0x42a5f5, 0x90caf9), // Светло-синий
            "❤️" => isDark
                ? (0x880e4f, 0xc2185b) // Тёмно-розовый
                : (0xf48fb1, 0xf8bbd0), // Светло-розовый
            "🎉" => isDark
                ? (0xe65100, 0xf57c00) // Тёмно-оранжевый
                : (0xffb74d, 0xffcc80), // Светло-оранжевый
            "🌊" => isDark
                ? (0x004d73, 0x006994) // Глубокий океан
                : (0x4fc3f7, 0x81d4fa), // Светлый океан
            "🌸" => isDark
                ? (0xad1457, 0xd81b60) // Тёмно-розовый (роза)
                : (0xf06292, 0xf48fb1), // Светло-розовый (роза)
            "🌙" => isDark
                ? (0x311b92, 0x512da8) // Глубокий фиолетовый
                : (0x9575cd, 0xb39ddb), // Светло-фиолетовый
            "🔥" => isDark
                ? (0xbf360c, 0xe64a19) // Тёмный огненный
                : (0xff7043, 0xff8a65), // Светлый огненный
            "💚" => isDark
                ? (0x1b5e20, 0x2e7d32) // Тёмно-зелёный
                : (0x66bb6a, 0x81c784), // Светло-зелёный
            _ => isDark
                ? (0x0a3d5c, 0x1565a0) // Тёмно-синий по умолчанию
                : (0x42a5f5, 0x90caf9)  // Светло-синий по умолчанию
        };
    }
}
