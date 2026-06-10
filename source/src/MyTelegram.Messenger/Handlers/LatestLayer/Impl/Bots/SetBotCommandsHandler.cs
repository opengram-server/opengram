using MongoDB.Driver;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Set bot commands for the specified bot scope and language.
/// See <a href="https://corefork.telegram.org/method/bots.setBotCommands" />
///</summary>
internal sealed class SetBotCommandsHandler(
    IQueryProcessor queryProcessor,
    IMongoDatabase mongoDatabase,
    ILogger<SetBotCommandsHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestSetBotCommands, IBool>,
    Bots.ISetBotCommandsHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestSetBotCommands obj)
    {
        // Validate commands per Telegram spec
        if (obj.Commands.Count > 100)
        {
            throw new RpcException(new RpcError(400, "BOTS_TOO_MUCH"));
        }

        foreach (var botCommand in obj.Commands)
        {
            if (botCommand is TBotCommand cmd)
            {
                if (string.IsNullOrEmpty(cmd.Command) || cmd.Command.Length > 32)
                {
                    throw new RpcException(new RpcError(400, "BOT_COMMAND_INVALID"));
                }

                if (string.IsNullOrEmpty(cmd.Description) || cmd.Description.Length > 256)
                {
                    throw new RpcException(new RpcError(400, "BOT_COMMAND_DESCRIPTION_INVALID"));
                }
            }
        }

        // Convert to domain model
        var commands = obj.Commands
            .OfType<TBotCommand>()
            .Select(c => new BotCommand(c.Command, c.Description))
            .ToList();

        // Persist to BotReadModel
        var collection = mongoDatabase.GetCollection<BotReadModel>("ReadModel-BotReadModel");
        var filter = Builders<BotReadModel>.Filter.Eq(b => b.UserId, input.UserId);
        var update = Builders<BotReadModel>.Update.Set(b => b.Commands, commands);

        var result = await collection.UpdateOneAsync(filter, update);

        if (result.MatchedCount == 0)
        {
            logger.LogWarning("SetBotCommands: Bot not found for UserId={UserId}", input.UserId);
        }
        else
        {
            logger.LogDebug("SetBotCommands: Updated {Count} commands for UserId={UserId}", commands.Count, input.UserId);
        }

        return new TBoolTrue();
    }
}
