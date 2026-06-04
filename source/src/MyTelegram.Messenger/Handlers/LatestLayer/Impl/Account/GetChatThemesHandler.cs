// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Get all available chat themes
/// See <a href="https://corefork.telegram.org/method/account.getChatThemes" />
///</summary>
internal sealed class GetChatThemesHandler(
    IQueryProcessor queryProcessor,
    IObjectMapper objectMapper,
    ILogger<GetChatThemesHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestGetChatThemes, MyTelegram.Schema.Account.IThemes>,
    Account.IGetChatThemesHandler
{
    protected override async Task<MyTelegram.Schema.Account.IThemes> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestGetChatThemes obj)
    {
        logger.LogInformation("GetChatThemesHandler: Starting, client hash={ClientHash}", obj.Hash);
        
        // Get all available chat themes
        var query = new GetAllChatThemesQuery();
        var themes = await queryProcessor.ProcessAsync(query, CancellationToken.None);

        logger.LogInformation("GetChatThemesHandler: Got {Count} themes from query", themes?.Count ?? 0);

        if (themes == null || themes.Count == 0)
        {
            logger.LogWarning("GetChatThemesHandler: No themes found, returning empty");
            return new TThemes
            {
                Hash = 0,
                Themes = new TVector<ITheme>()
            };
        }

        // Calculate hash
        var hash = CalculateHash(themes);
        
        logger.LogInformation("GetChatThemesHandler: Calculated hash={Hash}, client hash={ClientHash}", hash, obj.Hash);
        
        // Check if not modified
        if (obj.Hash == hash)
        {
            logger.LogInformation("GetChatThemesHandler: Hash matches, returning NotModified");
            return new TThemesNotModified();
        }

        // Collect all document IDs from themes and their settings
        var documentIds = new HashSet<long>();
        foreach (var theme in themes)
        {
            if (theme.DocumentId.HasValue)
            {
                documentIds.Add(theme.DocumentId.Value);
            }
            if (theme.Settings != null)
            {
                foreach (var setting in theme.Settings)
                {
                    if (setting.WallPaper?.DocumentId.HasValue == true)
                    {
                        documentIds.Add(setting.WallPaper.DocumentId.Value);
                    }
                }
            }
        }

        // Fetch all documents in one batch
        var documents = new Dictionary<long, IDocument>();
        foreach (var docId in documentIds)
        {
            var docQuery = new GetDocumentByIdQuery(docId);
            var doc = await queryProcessor.ProcessAsync(docQuery, CancellationToken.None);
            if (doc != null)
            {
                documents[docId] = objectMapper.Map<IDocumentReadModel, TDocument>(doc);
            }
        }

        // Convert themes to TTheme
        var themeList = new TVector<ITheme>();
        foreach (var theme in themes)
        {
            IDocument? themeDocument = null;
            if (theme.DocumentId.HasValue && documents.TryGetValue(theme.DocumentId.Value, out var doc))
            {
                themeDocument = doc;
            }

            var ttheme = new TTheme
            {
                Id = theme.Id,
                AccessHash = theme.AccessHash,
                Slug = theme.Slug,
                Title = theme.Title,
                ForChat = theme.ForChat,
                Default = theme.Default,
                Emoticon = theme.Emoticon,
                Document = themeDocument,
                Settings = ConvertSettings(theme.Settings, documents)
            };
            
            logger.LogInformation("GetChatThemesHandler: Adding theme Id={Id}, Emoticon={Emoticon}, ForChat={ForChat}, SettingsCount={SettingsCount}", 
                ttheme.Id, ttheme.Emoticon, ttheme.ForChat, ttheme.Settings?.Count ?? 0);
            
            themeList.Add(ttheme);
        }

        var result = new TThemes
        {
            Hash = hash,
            Themes = themeList
        };
        
        logger.LogInformation("GetChatThemesHandler: Returning {Count} themes with hash={Hash}", themeList.Count, hash);
        
        return result;
    }

    private static long CalculateHash(IReadOnlyList<Theme> themes)
    {
        if (themes.Count == 0) return 0;

        long hash = 0;
        foreach (var theme in themes)
        {
            hash = hash * 31 + theme.Id;
        }
        return hash;
    }

    private static TVector<IThemeSettings>? ConvertSettings(List<ThemeSettings> settings, Dictionary<long, IDocument> documents)
    {
        if (settings == null || settings.Count == 0)
        {
            return null;
        }

        var result = new TVector<IThemeSettings>();
        foreach (var setting in settings)
        {
            result.Add(new TThemeSettings
            {
                MessageColorsAnimated = setting.MessageColorsAnimated,
                BaseTheme = setting.BaseTheme, // Already IBaseTheme, no conversion needed
                AccentColor = setting.AccentColor,
                OutboxAccentColor = setting.OutboxAccentColor,
                MessageColors = setting.MessageColors != null ? new TVector<int>(setting.MessageColors) : null,
                Wallpaper = setting.WallPaper != null ? ConvertWallpaper(setting.WallPaper, documents) : null
            });
        }
        return result;
    }

    private static IBaseTheme ConvertBaseTheme(long baseTheme)
    {
        // Map baseTheme ID to appropriate TBaseTheme type
        // Based on Telegram's base theme IDs
        return baseTheme switch
        {
            1 => new TBaseThemeClassic(),
            2 => new TBaseThemeDay(),
            3 => new TBaseThemeNight(),
            4 => new TBaseThemeTinted(),
            5 => new TBaseThemeArctic(),
            _ => new TBaseThemeDay() // Default
        };
    }

    private static IWallPaper? ConvertWallpaper(WallPaper wallpaper, Dictionary<long, IDocument> documents)
    {
        if (wallpaper.DocumentId.HasValue)
        {
            IDocument? document = null;
            if (documents.TryGetValue(wallpaper.DocumentId.Value, out var doc))
            {
                document = doc;
            }
            else
            {
                // Fallback to empty document if not found
                document = new TDocumentEmpty { Id = wallpaper.DocumentId.Value };
            }

            return new TWallPaper
            {
                Id = wallpaper.Id,
                AccessHash = wallpaper.AccessHash,
                Slug = wallpaper.Slug,
                Default = wallpaper.Default,
                Pattern = wallpaper.Pattern,
                Dark = wallpaper.Dark,
                Document = document,
                Settings = wallpaper.Settings != null ? ConvertWallpaperSettings(wallpaper.Settings) : null
            };
        }

        return new TWallPaperNoFile
        {
            Id = wallpaper.Id,
            Default = wallpaper.Default,
            Dark = wallpaper.Dark,
            Settings = wallpaper.Settings != null ? ConvertWallpaperSettings(wallpaper.Settings) : null
        };
    }

    private static TWallPaperSettings? ConvertWallpaperSettings(WallPaperSettings settings)
    {
        return new TWallPaperSettings
        {
            Blur = settings.Blur,
            Motion = settings.Motion,
            BackgroundColor = settings.BackgroundColor,
            SecondBackgroundColor = settings.SecondBackgroundColor,
            ThirdBackgroundColor = settings.ThirdBackgroundColor,
            FourthBackgroundColor = settings.FourthBackgroundColor,
            Intensity = settings.Intensity,
            Rotation = settings.Rotation,
            Emoticon = settings.Emoticon
        };
    }
}
