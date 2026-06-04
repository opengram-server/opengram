// ReSharper disable All

using MyTelegram.Domain.Aggregates.Wallpaper;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Install/uninstall <a href="https://corefork.telegram.org/api/wallpapers">wallpaper</a>
/// <para>Possible errors</para>
/// Code Type Description
/// 400 WALLPAPER_INVALID The specified wallpaper is invalid.
/// See <a href="https://corefork.telegram.org/method/account.saveWallPaper" />
///</summary>
internal sealed class SaveWallPaperHandler(
    ICommandBus commandBus,
    IQueryProcessor queryProcessor)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestSaveWallPaper, IBool>,
    Account.ISaveWallPaperHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestSaveWallPaper obj)
    {
        // Get wallpaper ID from input
        var wallpaperId = GetWallpaperId(obj.Wallpaper);
        if (!wallpaperId.HasValue)
        {
            RpcErrors.RpcErrors400.WallpaperInvalid.ThrowRpcError();
        }

        // Verify wallpaper exists
        var query = new GetWallpaperByIdQuery(wallpaperId.Value);
        var wallpaper = await queryProcessor.ProcessAsync(query, CancellationToken.None);
        
        if (wallpaper == null || wallpaper.IsDeleted)
        {
            RpcErrors.RpcErrors400.WallpaperInvalid.ThrowRpcError();
        }

        // Convert settings
        var settings = obj.Settings != null ? ToWallPaperSettings(obj.Settings) : null;

        // Execute command
        var command = new SaveWallpaperCommand(
            WallpaperId.With(wallpaper.Id),
            input.ToRequestInfo(),
            input.UserId,
            obj.Unsave,
            settings!
        );

        await commandBus.PublishAsync(command, CancellationToken.None);

        return new TBoolTrue();
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
