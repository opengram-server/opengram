using MongoDB.Driver;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Sets the menu button action for a given user or for all users.
/// See <a href="https://corefork.telegram.org/method/bots.setBotMenuButton" />
///</summary>
internal sealed class SetBotMenuButtonHandler(
    IMongoDatabase mongoDatabase,
    ILogger<SetBotMenuButtonHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestSetBotMenuButton, IBool>,
    Bots.ISetBotMenuButtonHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestSetBotMenuButton obj)
    {
        var collection = mongoDatabase.GetCollection<BotReadModel>("ReadModel-BotReadModel");
        var filter = Builders<BotReadModel>.Filter.Eq(b => b.UserId, input.UserId);

        string? miniAppUrl = null;

        if (obj.Button is TBotMenuButton menuButton)
        {
            miniAppUrl = menuButton.Url;
        }
        // TBotMenuButtonDefault or TBotMenuButtonCommands → clear the URL

        var update = Builders<BotReadModel>.Update.Set(b => b.MiniAppUrl, miniAppUrl);
        var result = await collection.UpdateOneAsync(filter, update);

        if (result.MatchedCount == 0)
        {
            logger.LogWarning("SetBotMenuButton: Bot not found for UserId={UserId}", input.UserId);
        }
        else
        {
            logger.LogDebug("SetBotMenuButton: Updated menu button for UserId={UserId}, Url={Url}",
                input.UserId, miniAppUrl ?? "(cleared)");
        }

        return new TBoolTrue();
    }
}
