using MongoDB.Driver;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Clear bot commands for the specified bot scope and language.
/// See <a href="https://corefork.telegram.org/method/bots.resetBotCommands" />
///</summary>
internal sealed class ResetBotCommandsHandler(
    IMongoDatabase mongoDatabase,
    ILogger<ResetBotCommandsHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestResetBotCommands, IBool>,
    Bots.IResetBotCommandsHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestResetBotCommands obj)
    {
        var collection = mongoDatabase.GetCollection<BotReadModel>("ReadModel-BotReadModel");
        var filter = Builders<BotReadModel>.Filter.Eq(b => b.UserId, input.UserId);
        var update = Builders<BotReadModel>.Update.Set(b => b.Commands, new List<BotCommand>());

        var result = await collection.UpdateOneAsync(filter, update);

        if (result.MatchedCount > 0)
        {
            logger.LogDebug("ResetBotCommands: Cleared commands for UserId={UserId}", input.UserId);
        }

        return new TBoolTrue();
    }
}
