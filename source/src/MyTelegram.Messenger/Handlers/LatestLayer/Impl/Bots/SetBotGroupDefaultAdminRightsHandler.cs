using MongoDB.Driver;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Set the default suggested admin rights for bots being added as admins to groups.
/// See <a href="https://corefork.telegram.org/method/bots.setBotGroupDefaultAdminRights" />
///</summary>
internal sealed class SetBotGroupDefaultAdminRightsHandler(
    IMongoDatabase mongoDatabase,
    ILogger<SetBotGroupDefaultAdminRightsHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestSetBotGroupDefaultAdminRights, IBool>,
    Bots.ISetBotGroupDefaultAdminRightsHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestSetBotGroupDefaultAdminRights obj)
    {
        // Use the Flags field which has all admin rights packed as bits
        int adminRightsFlags = 0;
        if (obj.AdminRights is TChatAdminRights rights)
        {
            adminRightsFlags = rights.Flags;
        }

        var collection = mongoDatabase.GetCollection<BotReadModel>("ReadModel-BotReadModel");
        var filter = Builders<BotReadModel>.Filter.Eq(b => b.UserId, input.UserId);
        var update = Builders<BotReadModel>.Update.Set(b => b.ChatAdminRights, adminRightsFlags);

        var result = await collection.UpdateOneAsync(filter, update);

        if (result.MatchedCount > 0)
        {
            logger.LogDebug("SetBotGroupDefaultAdminRights: Updated for UserId={UserId}, Flags=0x{Flags:X}",
                input.UserId, adminRightsFlags);
        }

        return new TBoolTrue();
    }
}
