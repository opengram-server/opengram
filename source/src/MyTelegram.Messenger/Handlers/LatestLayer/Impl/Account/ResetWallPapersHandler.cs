// ReSharper disable All

using MyTelegram.Domain.Aggregates.Wallpaper;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Delete all installed <a href="https://corefork.telegram.org/api/wallpapers">wallpapers</a>, reverting to the default wallpaper set.
/// See <a href="https://corefork.telegram.org/method/account.resetWallPapers" />
///</summary>
internal sealed class ResetWallPapersHandler(
    IQueryProcessor queryProcessor,
    ICommandBus commandBus)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestResetWallPapers, IBool>,
    Account.IResetWallPapersHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestResetWallPapers obj)
    {
        // Get all user's wallpapers
        var query = new GetUserWallpapersQuery(input.UserId);
        var userWallpapers = await queryProcessor.ProcessAsync(query, CancellationToken.None);

        if (userWallpapers != null && userWallpapers.Count > 0)
        {
            // Delete all wallpapers for this user
            foreach (var wallpaper in userWallpapers)
            {
                var command = new DeleteWallpaperCommand(
                    WallpaperId.With(wallpaper.Id),
                    input.ToRequestInfo(),
                    input.UserId
                );

                await commandBus.PublishAsync(command, CancellationToken.None);
            }
        }

        return new TBoolTrue();
    }
}
