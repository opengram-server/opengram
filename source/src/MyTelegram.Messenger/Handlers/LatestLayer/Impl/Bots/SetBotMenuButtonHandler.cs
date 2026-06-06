namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Sets the menu button action for a given user or for all users; only available for bots.
/// See <a href="https://corefork.telegram.org/method/bots.setBotMenuButton" />
///</summary>
internal sealed class SetBotMenuButtonHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestSetBotMenuButton, IBool>,
    Bots.ISetBotMenuButtonHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestSetBotMenuButton obj)
    {
        // Accept the menu button configuration
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
