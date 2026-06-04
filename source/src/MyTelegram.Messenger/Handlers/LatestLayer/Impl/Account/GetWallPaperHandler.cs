// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Returns info on a certain wallpaper
/// See <a href="https://corefork.telegram.org/method/account.getWallPaper" />
///</summary>
internal sealed class GetWallPaperHandler(
    IQueryProcessor queryProcessor,
    IObjectMapper objectMapper)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestGetWallPaper, MyTelegram.Schema.IWallPaper>,
    Account.IGetWallPaperHandler
{
    protected override async Task<MyTelegram.Schema.IWallPaper> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestGetWallPaper obj)
    {
        long? wallpaperId = null;

        // Get wallpaper ID from input
        if (obj.Wallpaper is TInputWallPaper wp)
        {
            wallpaperId = wp.Id;
        }
        else if (obj.Wallpaper is TInputWallPaperSlug slug)
        {
            // Find wallpaper by slug
            var slugQuery = new GetWallpaperBySlugQuery(slug.Slug);
            var wallpaperBySlug = await queryProcessor.ProcessAsync(slugQuery, CancellationToken.None);
            wallpaperId = wallpaperBySlug?.WallpaperId;
        }
        else if (obj.Wallpaper is TInputWallPaperNoFile noFile)
        {
            wallpaperId = noFile.Id;
        }

        if (!wallpaperId.HasValue)
        {
            RpcErrors.RpcErrors400.WallpaperInvalid.ThrowRpcError();
        }

        // Get wallpaper
        var query = new GetWallpaperByIdQuery(wallpaperId.Value);
        var wallpaper = await queryProcessor.ProcessAsync(query, CancellationToken.None);

        if (wallpaper == null || wallpaper.IsDeleted)
        {
            RpcErrors.RpcErrors400.WallpaperNotFound.ThrowRpcError();
        }

        // Fetch document if wallpaper has one
        IDocument? document = null;
        if (wallpaper.DocumentId.HasValue)
        {
            var docQuery = new GetDocumentByIdQuery(wallpaper.DocumentId.Value);
            var doc = await queryProcessor.ProcessAsync(docQuery, CancellationToken.None);
            if (doc != null)
            {
                document = objectMapper.Map<IDocumentReadModel, TDocument>(doc);
            }
            else
            {
                document = new TDocumentEmpty { Id = wallpaper.DocumentId.Value };
            }
        }

        // Convert wallpaper
        if (wallpaper.DocumentId.HasValue)
        {
            return new TWallPaper
            {
                Id = wallpaper.WallpaperId,
                AccessHash = wallpaper.AccessHash,
                Slug = wallpaper.Slug,
                Default = wallpaper.Default,
                Pattern = wallpaper.Pattern,
                Dark = wallpaper.Dark,
                Creator = false,
                Document = document!,
                Settings = wallpaper.Settings != null ? ConvertWallPaperSettings(wallpaper.Settings) : null
            };
        }

        return new TWallPaperNoFile
        {
            Id = wallpaper.WallpaperId,
            Default = wallpaper.Default,
            Dark = wallpaper.Dark,
            Settings = wallpaper.Settings != null ? ConvertWallPaperSettings(wallpaper.Settings) : null
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
