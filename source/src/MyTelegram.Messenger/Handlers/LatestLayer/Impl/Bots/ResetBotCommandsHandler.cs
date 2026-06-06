namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Clear bot commands for the specified bot scope and language code.
/// See <a href="https://corefork.telegram.org/method/bots.resetBotCommands" />
///</summary>
internal sealed class ResetBotCommandsHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestResetBotCommands, IBool>,
    Bots.IResetBotCommandsHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestResetBotCommands obj)
    {
        // Bot commands are managed via BotFather in the Telegram ecosystem.
        // In the self-hosted context, acknowledge the reset request.
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
