using MongoDB.Driver;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Contacts;

///<summary>
/// Import a contact token, to add a user to our contact list.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 IMPORT_TOKEN_INVALID The specified token is invalid.
/// See <a href="https://corefork.telegram.org/method/contacts.importContactToken" />
///</summary>
internal sealed class ImportContactTokenHandler(
    IUserConverterService userConverterService,
    IUserAppService userAppService,
    IMongoDatabase mongoDatabase,
    ILogger<ImportContactTokenHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Contacts.RequestImportContactToken, MyTelegram.Schema.IUser>,
        Contacts.IImportContactTokenHandler
{
    protected override async Task<IUser> HandleCoreAsync(IRequestInput input,
        RequestImportContactToken obj)
    {
        if (string.IsNullOrEmpty(obj.Token))
        {
            throw new RpcException(new RpcError(400, "IMPORT_TOKEN_INVALID"));
        }

        // Look up the token in the same collection used by ExportContactTokenHandler
        var collection = mongoDatabase.GetCollection<ContactTokenDocument>("ContactTokens");
        var filter = Builders<ContactTokenDocument>.Filter.Eq(d => d.Token, obj.Token);
        var tokenDoc = await collection.Find(filter).FirstOrDefaultAsync();

        if (tokenDoc == null)
        {
            throw new RpcException(new RpcError(400, "IMPORT_TOKEN_INVALID"));
        }

        // Check expiry
        if (tokenDoc.ExpiresAt < DateTime.UtcNow)
        {
            throw new RpcException(new RpcError(400, "IMPORT_TOKEN_INVALID"));
        }

        var targetUserId = tokenDoc.UserId;

        // Cannot import your own token
        if (targetUserId == input.UserId)
        {
            throw new RpcException(new RpcError(400, "CONTACT_ID_INVALID"));
        }

        // Get the user
        var userReadModel = await userAppService.GetAsync(targetUserId);

        logger.LogDebug("ImportContactToken: User {UserId} imported token for target {TargetUserId}",
            input.UserId, targetUserId);

        return userConverterService.ToUser(input, userReadModel);
    }
}

// ContactTokenDocument is defined in ExportContactTokenHandler.cs
