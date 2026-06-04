// ReSharper disable All

using MyTelegram.Domain.Aggregates.Wallpaper;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Create and upload a new <a href="https://corefork.telegram.org/api/wallpapers">wallpaper</a>
/// <para>Possible errors</para>
/// Code Type Description
/// 400 WALLPAPER_FILE_INVALID The specified wallpaper file is invalid.
/// 400 WALLPAPER_MIME_INVALID The specified wallpaper MIME type is invalid.
/// See <a href="https://corefork.telegram.org/method/account.uploadWallPaper" />
///</summary>
internal sealed class UploadWallPaperHandler(
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IRandomHelper randomHelper,
    IAccessHashHelper accessHashHelper,
    IPhotoAppService photoAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestUploadWallPaper, MyTelegram.Schema.IWallPaper>,
        Account.IUploadWallPaperHandler
{
    protected override async Task<MyTelegram.Schema.IWallPaper> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestUploadWallPaper obj)
    {
        // Validate MIME type
        var validMimeTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
        if (!validMimeTypes.Contains(obj.MimeType.ToLowerInvariant()))
        {
            RpcErrors.RpcErrors400.WallpaperMimeInvalid.ThrowRpcError();
        }

        // Upload file and create document
        var uploadedFile = obj.File;
        long? documentId = null;

        if (uploadedFile != null)
        {
            // Create document from uploaded file
            var docId = await idGenerator.NextLongIdAsync(IdType.DocumentId, 0, 1, CancellationToken.None);
            // Here you would typically save the file and create a document
            // For now, we'll just use the generated ID
            documentId = docId;
        }

        // Generate unique slug
        var slug = $"custom_{Random.Shared.NextInt64():x}";

        // Create wallpaper command
        var wallpaperId = WallpaperId.New;
        var createdWallpaperId = await idGenerator.NextLongIdAsync(IdType.WallPaperId, 0, 1, CancellationToken.None);
        var accessHash = Random.Shared.NextInt64();
        
        var command = new CreateWallpaperCommand(
            wallpaperId,
            input.ToRequestInfo(),
            createdWallpaperId,
            input.UserId,
            accessHash,
            slug,
            documentId,
            false, // isDefault
            false, // isPattern
            false, // isDark
            obj.Settings != null ? ToWallPaperSettings(obj.Settings) : null
        );

        await commandBus.PublishAsync(command, CancellationToken.None);

        // Save access hash
        accessHashHelper.AddAccessHash(createdWallpaperId, accessHash);

        // Return wallpaper
        var wallpaper = new TWallPaper
        {
            Id = createdWallpaperId,
            AccessHash = accessHash,
            Slug = slug,
            Creator = true,
            Document = documentId.HasValue ? new TDocumentEmpty { Id = documentId.Value } : new TDocumentEmpty { Id = 0 },
            Settings = obj.Settings
        };

        return wallpaper;
    }

    private static WallPaperSettings ToWallPaperSettings(IWallPaperSettings settings)
    {
        if (settings is TWallPaperSettings s)
        {
            return new WallPaperSettings(
                s.Blur,
                s.Motion,
                s.BackgroundColor,
                s.SecondBackgroundColor,
                s.ThirdBackgroundColor,
                s.FourthBackgroundColor,
                s.Intensity,
                s.Rotation,
                s.Emoticon
            );
        }

        return new WallPaperSettings(false, false, null, null, null, null, null, null, null);
    }
}
