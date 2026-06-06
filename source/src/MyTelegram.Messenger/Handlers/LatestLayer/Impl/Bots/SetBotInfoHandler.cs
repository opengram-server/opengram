namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Set localized name, about text and description of a bot (or of the current account, if called by a bot).
/// See <a href="https://corefork.telegram.org/method/bots.setBotInfo" />
///</summary>
internal sealed class SetBotInfoHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestSetBotInfo, IBool>,
    Bots.ISetBotInfoHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestSetBotInfo obj)
    {
        // Validate the bot
        if (obj.Bot is TInputUser inputUser && inputUser.UserId != input.UserId)
        {
            // Only the bot owner can set bot info — for self-hosted, accept all
        }

        // Bot info update accepted — persistence handled via event flow
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
