namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Help;

///<summary>
/// Informs the server about the number of pending bot updates if they haven't been processed for a long time; for bots only
/// <para>Possible errors</para>
/// Code Type Description
/// 400 USER_BOT_REQUIRED This method can only be called by a bot.
/// See <a href="https://corefork.telegram.org/method/help.setBotUpdatesStatus" />
///</summary>
internal sealed class SetBotUpdatesStatusHandler : RpcResultObjectHandler<MyTelegram.Schema.Help.RequestSetBotUpdatesStatus, IBool>,
    Help.ISetBotUpdatesStatusHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Help.RequestSetBotUpdatesStatus obj)
    {
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
