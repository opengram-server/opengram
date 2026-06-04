namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Returns a list of available <a href="https://corefork.telegram.org/api/wallpapers">wallpapers</a>.
/// See <a href="https://corefork.telegram.org/method/account.getWallPapers" />
///</summary>
internal sealed class GetWallPapersHandler(
    IQueryProcessor queryProcessor,
    IObjectMapper objectMapper) 
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestGetWallPapers, MyTelegram.Schema.Account.IWallPapers>,
    Account.IGetWallPapersHandler
{
    protected override async Task<MyTelegram.Schema.Account.IWallPapers> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestGetWallPapers obj)
    {
        // Get ALL default wallpapers (image and pattern only, NOT fill wallpapers)
        // Fill wallpapers are stored locally by clients, not synced via API
        var allWallpapersQuery = new GetAllWallpapersQuery();
        var userWallpapers = await queryProcessor.ProcessAsync(allWallpapersQuery, CancellationToken.None);
        
        if (userWallpapers == null || userWallpapers.Count == 0)
        {
            return new TWallPapers
            {
                Hash = 0,
                Wallpapers = new TVector<IWallPaper>()
            };
        }

        // Calculate hash
        var hash = CalculateHash(userWallpapers.ToList());
        
        // Check if not modified
        if (obj.Hash == hash)
        {
            return new TWallPapersNotModified();
        }

        // Convert to wallpaper list
        var wallpapers = new TVector<IWallPaper>();
        foreach (var wp in userWallpapers)
        {
            // For fill wallpapers (no document), use TWallPaperNoFile
            if (!wp.DocumentId.HasValue)
            {
                wallpapers.Add(new TWallPaperNoFile
                {
                    Id = wp.WallpaperId,
                    Default = wp.Default,
                    Dark = wp.Dark,
                    Settings = wp.Settings != null ? ConvertWallPaperSettings(wp.Settings) : null
                });
            }
            else
            {
                // For image/pattern wallpapers, fetch document
                IDocument? document = null;
                var docQuery = new GetDocumentByIdQuery(wp.DocumentId.Value);
                var doc = await queryProcessor.ProcessAsync(docQuery, CancellationToken.None);
                if (doc != null)
                {
                    document = objectMapper.Map<IDocumentReadModel, TDocument>(doc);
                }
                else
                {
                    document = new TDocumentEmpty { Id = wp.DocumentId.Value };
                }

                wallpapers.Add(new TWallPaper
                {
                    Id = wp.WallpaperId,
                    AccessHash = wp.AccessHash,
                    Slug = wp.Slug,
                    Default = wp.Default,
                    Pattern = wp.Pattern,
                    Dark = wp.Dark,
                    Creator = false,
                    Document = document,
                    Settings = wp.Settings != null ? ConvertWallPaperSettings(wp.Settings) : null
                });
            }
        }

        return new TWallPapers
        {
            Hash = hash,
            Wallpapers = wallpapers
        };
    }

    private static long CalculateHash(IList<IWallpaperReadModel> wallpapers)
    {
        if (wallpapers.Count == 0) return 0;
        
        long hash = 0;
        foreach (var wp in wallpapers)
        {
            hash = hash * 31 + wp.WallpaperId;
        }
        return hash;
    }


    private static TWallPaperSettings ConvertWallPaperSettings(WallPaperSettings settings)
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
