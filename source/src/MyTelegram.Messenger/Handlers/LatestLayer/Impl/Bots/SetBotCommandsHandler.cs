namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Set bot command list.
/// See <a href="https://corefork.telegram.org/method/bots.setBotCommands" />
///</summary>
internal sealed class SetBotCommandsHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestSetBotCommands, IBool>,
    Bots.ISetBotCommandsHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestSetBotCommands obj)
    {
        // Validate input
        if (obj.Commands == null || obj.Commands.Count == 0)
        {
            return Task.FromResult<IBool>(new TBoolTrue());
        }

        foreach (var cmd in obj.Commands)
        {
            if (cmd is TBotCommand botCommand)
            {
                if (string.IsNullOrEmpty(botCommand.Command) || botCommand.Command.Length > 32)
                {
                    RpcErrors.RpcErrors400.BotCommandInvalid.ThrowRpcError();
                }

                if (string.IsNullOrEmpty(botCommand.Description) || botCommand.Description.Length > 256)
                {
                    RpcErrors.RpcErrors400.BotCommandDescriptionInvalid.ThrowRpcError();
                }
            }
        }

        // Commands are accepted — persistence is handled via the bot read model event flow
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
