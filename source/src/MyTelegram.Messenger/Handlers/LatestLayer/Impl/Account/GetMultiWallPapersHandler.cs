// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Get info about multiple <a href="https://corefork.telegram.org/api/wallpapers">wallpapers</a>
/// <para>Possible errors</para>
/// Code Type Description
/// 400 WALLPAPER_INVALID The specified wallpaper is invalid.
/// See <a href="https://corefork.telegram.org/method/account.getMultiWallPapers" />
///</summary>
internal sealed class GetMultiWallPapersHandler(
    IQueryProcessor queryProcessor,
    IObjectMapper objectMapper)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestGetMultiWallPapers, TVector<MyTelegram.Schema.IWallPaper>>,
    Account.IGetMultiWallPapersHandler
{
    protected override async Task<TVector<MyTelegram.Schema.IWallPaper>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestGetMultiWallPapers obj)
    {
        var result = new TVector<IWallPaper>();
        var wallpapers = new List<IWallpaperReadModel>();

        // Collect all wallpapers first
        foreach (var inputWallpaper in obj.Wallpapers)
        {
            var wallpaperId = GetWallpaperId(inputWallpaper);
            if (!wallpaperId.HasValue)
            {
                continue;
            }

            var query = new GetWallpaperByIdQuery(wallpaperId.Value);
            var wallpaper = await queryProcessor.ProcessAsync(query, CancellationToken.None);

            if (wallpaper != null && !wallpaper.IsDeleted)
            {
                wallpapers.Add(wallpaper);
            }
        }

        // Collect all document IDs
        var documentIds = new HashSet<long>();
        foreach (var wp in wallpapers)
        {
            if (wp.DocumentId.HasValue)
            {
                documentIds.Add(wp.DocumentId.Value);
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

        // Convert wallpapers with documents
        foreach (var wp in wallpapers)
        {
            result.Add(ConvertWallpaper(wp, documents));
        }

        return result;
    }

    private static long? GetWallpaperId(IInputWallPaper wallpaper)
    {
        return wallpaper switch
        {
            TInputWallPaper w => w.Id,
            TInputWallPaperNoFile w => w.Id,
            _ => null
        };
    }

    private static IWallPaper ConvertWallpaper(IWallpaperReadModel wp, Dictionary<long, IDocument> documents)
    {
        if (wp.DocumentId.HasValue)
        {
            IDocument? document = null;
            if (documents.TryGetValue(wp.DocumentId.Value, out var doc))
            {
                document = doc;
            }
            else
            {
                document = new TDocumentEmpty { Id = wp.DocumentId.Value };
            }

            return new TWallPaper
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
            };
        }

        return new TWallPaperNoFile
        {
            Id = wp.WallpaperId,
            Default = wp.Default,
            Dark = wp.Dark,
            Settings = wp.Settings != null ? ConvertWallPaperSettings(wp.Settings) : null
        };
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
