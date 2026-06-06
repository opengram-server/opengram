using MongoDB.Driver;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Set localized name, about text and description of a bot (or of the current account, if called by a bot).
/// See <a href="https://corefork.telegram.org/method/bots.setBotInfo" />
///</summary>
internal sealed class SetBotInfoHandler(
    IMongoDatabase mongoDatabase,
    ILogger<SetBotInfoHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestSetBotInfo, IBool>,
    Bots.ISetBotInfoHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestSetBotInfo obj)
    {
        // Determine target bot user ID
        long botUserId = input.UserId;
        if (obj.Bot is TInputUser inputUser)
        {
            botUserId = inputUser.UserId;
        }

        var collection = mongoDatabase.GetCollection<BotReadModel>("ReadModel-BotReadModel");
        var filter = Builders<BotReadModel>.Filter.Eq(b => b.UserId, botUserId);

        var updateDefinitions = new List<UpdateDefinition<BotReadModel>>();

        if (obj.Name != null)
        {
            if (obj.Name.Length > 64)
            {
                throw new RpcException(new TRpcError { ErrorCode = 400, ErrorMessage = "NAME_NOT_MODIFIED" });
            }
            updateDefinitions.Add(Builders<BotReadModel>.Update.Set(b => b.BotName, obj.Name));
        }

        if (obj.About != null)
        {
            if (obj.About.Length > 120)
            {
                throw new RpcException(new TRpcError { ErrorCode = 400, ErrorMessage = "ABOUT_TOO_LONG" });
            }
            updateDefinitions.Add(Builders<BotReadModel>.Update.Set(b => b.About, obj.About));
        }

        if (obj.Description != null)
        {
            if (obj.Description.Length > 512)
            {
                throw new RpcException(new TRpcError { ErrorCode = 400, ErrorMessage = "DESCRIPTION_TOO_LONG" });
            }
            updateDefinitions.Add(Builders<BotReadModel>.Update.Set(b => b.Description, obj.Description));
        }

        if (updateDefinitions.Count > 0)
        {
            var update = Builders<BotReadModel>.Update.Combine(updateDefinitions);
            var result = await collection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                logger.LogWarning("SetBotInfo: Bot not found for UserId={BotUserId}", botUserId);
                throw new RpcException(new TRpcError { ErrorCode = 400, ErrorMessage = "BOT_INVALID" });
            }

            logger.LogDebug("SetBotInfo: Updated info for BotUserId={BotUserId}", botUserId);
        }

        return new TBoolTrue();
    }
}
