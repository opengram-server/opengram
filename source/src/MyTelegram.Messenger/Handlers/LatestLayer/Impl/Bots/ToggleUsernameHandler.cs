namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Activate or deactivate a purchased collectible username associated to a bot.
/// See <a href="https://corefork.telegram.org/method/bots.toggleUsername" />
///</summary>
internal sealed class ToggleUsernameHandler(
    ICommandBus commandBus)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestToggleUsername, IBool>,
        Bots.IToggleUsernameHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestToggleUsername obj)
    {
        // Bot username toggle - requires bot ownership verification and collectible username infrastructure
        throw new RpcException(new RpcError(400, "BOT_INVALID"));
    }
}
