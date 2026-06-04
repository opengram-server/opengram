using MyTelegram.Domain.Shared.BotApi;
using MyTelegram.ReadModel.MongoDB;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.Schema;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MyTelegram.BotApi.Services;

/// <summary>
/// Processes incoming messages and creates bot updates
/// </summary>
public class BotUpdateProcessor(
    IMongoDatabase mongoDatabase,
    ILogger<BotUpdateProcessor> logger)
{
    /// <summary>
    /// Process new message and create bot update if recipient is a bot
    /// </summary>
    public async Task ProcessNewMessageAsync(
        IMessageReadModel message,
        IUser? sender,
        IChat? chat)
    {
        // Check if recipient is a bot - query directly from MongoDB
        var usersCollection = mongoDatabase.GetCollection<UserReadModel>("ReadModel-UserReadModel");
        var recipientUser = await usersCollection.Find(u => u.UserId == message.ToPeerId).FirstOrDefaultAsync();

        if (recipientUser == null || !recipientUser.Bot)
        {
            return; // Not a bot, skip
        }

        // Пропускаем сообщения, отправленные самим ботом, чтобы не зациклиться
        if (message.SenderUserId == recipientUser.UserId)
        {
            logger.LogDebug("Ignoring message from bot {BotId} to itself - preventing infinite loop", recipientUser.UserId);
            return;
        }

        logger.LogInformation("Processing message {MessageId} for bot {BotId} from user {SenderId}", 
            message.MessageId, recipientUser.UserId, message.SenderUserId);

        // Get bot info from MongoDB
        var botsCollection = mongoDatabase.GetCollection<MyTelegram.ReadModel.Impl.BotReadModel>("ReadModel-BotReadModel");
        var bot = await botsCollection.Find(b => b.UserId == recipientUser.UserId).FirstOrDefaultAsync();

        if (bot == null)
        {
            logger.LogWarning("Bot {BotId} not found", recipientUser.UserId);
            return;
        }

        // Create Bot API Update
        var botApiMessage = ConvertToBotApiMessage(message, sender, chat);
        var update = new BotApiUpdate
        {
            UpdateId = await GetNextUpdateIdAsync(bot.UserId),
            Message = botApiMessage
        };

        // Save update to database
        await SaveUpdateAsync(bot.UserId, update, "message");

        logger.LogInformation("Created update {UpdateId} for bot {BotId}", 
            update.UpdateId, bot.UserId);
    }

    /// <summary>
    /// Process callback query and create bot update
    /// </summary>
    public async Task ProcessCallbackQueryAsync(
        long botUserId,
        long queryId,
        long fromUserId,
        long chatId,
        int messageId,
        string data)
    {
        logger.LogInformation("Processing callback query {QueryId} for bot {BotId}", 
            queryId, botUserId);

        // Get bot info from MongoDB
        var botsCollection = mongoDatabase.GetCollection<MyTelegram.ReadModel.Impl.BotReadModel>("ReadModel-BotReadModel");
        var bot = await botsCollection.Find(b => b.UserId == botUserId).FirstOrDefaultAsync();

        if (bot == null)
        {
            logger.LogWarning("Bot {BotId} not found for callback query", botUserId);
            return;
        }

        // Get user info from MongoDB
        var usersCollection = mongoDatabase.GetCollection<UserReadModel>("ReadModel-UserReadModel");
        var user = await usersCollection.Find(u => u.UserId == fromUserId).FirstOrDefaultAsync();

        if (user == null)
        {
            return;
        }

        // Create callback query update
        var callbackQuery = new BotApiCallbackQuery
        {
            Id = queryId.ToString(),
            From = new BotApiUser
            {
                Id = user.UserId,
                IsBot = user.Bot,
                FirstName = user.FirstName ?? "",
                LastName = user.LastName,
                Username = user.UserName
            },
            ChatInstance = $"{chatId}_{messageId}",
            Data = data
        };

        var update = new BotApiUpdate
        {
            UpdateId = await GetNextUpdateIdAsync(botUserId),
            CallbackQuery = callbackQuery
        };

        // Save update
        await SaveUpdateAsync(botUserId, update, "callback_query");

        logger.LogInformation("Created callback query update {UpdateId} for bot {BotId}", 
            update.UpdateId, botUserId);
    }

    /// <summary>
    /// Get updates for bot (for getUpdates method)
    /// </summary>
    public async Task<List<BotApiUpdate>> GetUpdatesAsync(
        long botUserId,
        long offset,
        int limit)
    {
        // NOTE: Using simple document structure instead of ReadModel
        var collection = mongoDatabase.GetCollection<MongoDB.Bson.BsonDocument>("bot_updates");

        // Get updates starting from offset
        var filter = MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.And(
            MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.Eq("BotUserId", botUserId),
            MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.Gte("UpdateId", offset)
        );

        var docs = await collection
            .Find(filter)
            .Sort(MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Sort.Ascending("UpdateId"))
            .Limit(limit)
            .ToListAsync();

        // Convert BsonDocuments to BotApiUpdate
        var updates = new List<BotApiUpdate>();
        foreach (var doc in docs)
        {
            var updateBson = doc["Update"].AsBsonDocument;
            var updateJson = updateBson.ToJson();
            var update = System.Text.Json.JsonSerializer.Deserialize<BotApiUpdate>(updateJson);
            if (update != null)
            {
                updates.Add(update);
            }
        }

        // Mark as delivered
        if (docs.Count > 0)
        {
            var updateIds = docs.Select(x => x["UpdateId"].AsInt64).ToList();
            var updateFilter = MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.And(
                MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.Eq("BotUserId", botUserId),
                MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.In("UpdateId", updateIds)
            );

            var updateDefinition = MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Update
                .Set("IsDelivered", true)
                .Set("DeliveredAt", DateTime.UtcNow);

            await collection.UpdateManyAsync(updateFilter, updateDefinition);
        }

        return updates;
    }

    /// <summary>
    /// Delete old delivered updates (cleanup)
    /// </summary>
    public async Task CleanupOldUpdatesAsync(long botUserId, long confirmedOffset)
    {
        var collection = mongoDatabase.GetCollection<MongoDB.Bson.BsonDocument>("bot_updates");

        var filter = MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.And(
            MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.Eq("BotUserId", botUserId),
            MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.Lt("UpdateId", confirmedOffset),
            MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.Eq("IsDelivered", true)
        );

        var result = await collection.DeleteManyAsync(filter);
        
        if (result.DeletedCount > 0)
        {
            logger.LogInformation("Cleaned up {Count} old updates for bot {BotId}", 
                result.DeletedCount, botUserId);
        }
    }

    #region Private Methods

    private async Task<long> GetNextUpdateIdAsync(long botUserId)
    {
        // Update LastUpdateId in BotReadModel and return new value
        var botsCollection = mongoDatabase.GetCollection<MyTelegram.ReadModel.Impl.BotReadModel>("ReadModel-BotReadModel");
        
        var filter = MongoDB.Driver.Builders<MyTelegram.ReadModel.Impl.BotReadModel>.Filter.Eq(x => x.UserId, botUserId);
        var update = MongoDB.Driver.Builders<MyTelegram.ReadModel.Impl.BotReadModel>.Update.Inc(x => x.LastUpdateId, 1);
        
        var options = new FindOneAndUpdateOptions<MyTelegram.ReadModel.Impl.BotReadModel>
        {
            ReturnDocument = ReturnDocument.After
        };
        
        var bot = await botsCollection.FindOneAndUpdateAsync(filter, update, options);
        return bot?.LastUpdateId ?? 1;
    }

    private async Task SaveUpdateAsync(long botUserId, BotApiUpdate update, string updateType)
    {
        var collection = mongoDatabase.GetCollection<MongoDB.Bson.BsonDocument>("bot_updates");

        // Create simple document
        var doc = new MongoDB.Bson.BsonDocument
        {
            { "BotUserId", botUserId },
            { "UpdateId", update.UpdateId },
            { "UpdateType", updateType },
            { "Update", MongoDB.Bson.BsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(update)) },
            { "IsDelivered", false },
            { "CreatedAt", DateTime.UtcNow }
        };

        await collection.InsertOneAsync(doc);
    }

    private BotApiMessage ConvertToBotApiMessage(
        IMessageReadModel message,
        IUser? sender,
        IChat? chat)
    {
        var botApiMessage = new BotApiMessage
        {
            MessageId = message.MessageId,
            Date = message.Date,
            Text = message.Message,
            Chat = chat != null 
                ? BotApiConverter.ToBotApiChat(chat)
                : BotApiConverter.ToBotApiChat(message.ToPeerId, message.ToPeerType)
        };

        // Add sender info
        if (sender != null)
        {
            botApiMessage.From = BotApiConverter.ToBotApiUser(sender);
        }

        // Add entities
        if (message.Entities2 != null && message.Entities2.Count > 0)
        {
            botApiMessage.Entities = message.Entities2
                .Select(e => BotApiConverter.ToBotApiMessageEntity(e))
                .ToList();
        }

        return botApiMessage;
    }

    #endregion
}
