using MongoDB.Driver;
using MongoDB.Bson;
using MyTelegram.Domain.Shared;
using MyTelegram.Domain.Shared.BotApi;
using Microsoft.Extensions.Configuration;

namespace MyTelegram.BotApi.Services;

/// <summary>
/// Фоновый сервис, который опрашивает MongoDB на наличие новых сообщений,
/// адресованных ботам.
/// </summary>
public class BotMessagePollerService(
    IServiceProvider serviceProvider,
    ILogger<BotMessagePollerService> logger) : BackgroundService
{
    private readonly TimeSpan _pollInterval = TimeSpan.FromMilliseconds(100); // короткий интервал опроса для быстрого отклика
    private readonly HashSet<int> _processedMessageIds = new(); // уже обработанные сообщения, чтобы не дублировать

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Bot Message Poller Service starting...");

            // Даём остальным сервисам время на инициализацию
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);

            logger.LogInformation("Bot Message Poller Service started successfully");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var updatesManager = scope.ServiceProvider.GetRequiredService<IUpdatesManager>();
                
                // Подключение к MongoDB
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var connectionString = configuration["MongoDB:ConnectionString"];
                var databaseName = configuration["MongoDB:DatabaseName"];
                
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                
                // Берём всех активных ботов
                var botsCollection = database.GetCollection<BsonDocument>("ReadModel-BotReadModel");
                var bots = await botsCollection.Find(Builders<BsonDocument>.Filter.Eq("IsActive", true))
                    .ToListAsync(stoppingToken);
                
                if (bots.Count == 0)
                {
                    await Task.Delay(_pollInterval, stoppingToken);
                    continue;
                }
                
                // Идентификаторы ботов (UserId в MongoDB может быть Int32 или Int64)
                var botIds = bots.Select(b => {
                    var userId = b["UserId"];
                    return userId.IsInt32 ? (long)userId.AsInt32 : userId.AsInt64;
                }).ToList();
                logger.LogInformation("Polling for {Count} bots: {BotIds}", botIds.Count, string.Join(", ", botIds));

                // Ищем новые сообщения для любого из ботов.
                // Имя коллекции "ReadModel-MessageReadModel" задаётся соглашением EventFlow.
                var messagesCollection = database.GetCollection<BsonDocument>("ReadModel-MessageReadModel");

                // Берём последние 10 сообщений без фильтра по MessageId — дубликаты отсеиваем в памяти.
                // ToPeerId в MongoDB хранится как Long.
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("ToPeerType", (int)PeerType.User),
                    Builders<BsonDocument>.Filter.In("ToPeerId", botIds)
                );
                
                var messages = await messagesCollection.Find(filter)
                    .Sort(Builders<BsonDocument>.Sort.Descending("MessageId"))
                    .Limit(10)
                    .ToListAsync(stoppingToken);
                
                if (messages.Count > 0)
                {
                    logger.LogInformation("Found {Count} new messages for bot", messages.Count);
                }
                
                foreach (var msg in messages)
                {
                    var messageId = msg["MessageId"].AsInt32;
                    
                    // Пропускаем уже обработанные сообщения
                    if (_processedMessageIds.Contains(messageId))
                    {
                        continue;
                    }
                    
                    var senderUserId = msg["SenderUserId"].AsInt64;
                    var toPeerId = msg["ToPeerId"].AsInt64; // идентификатор бота
                    var text = msg.Contains("Message") ? msg["Message"].AsString : "";
                    var date = msg["Date"].AsInt32;
                    
                    // Пропускаем сообщения от самого бота, чтобы не получить бесконечный цикл
                    if (senderUserId == toPeerId)
                    {
                        logger.LogDebug("Skipping message from bot {BotId} to itself", toPeerId);
                        _processedMessageIds.Add(messageId);
                        continue;
                    }
                    
                    // Формируем обновление в формате Bot API
                    var update = new BotApiUpdate
                    {
                        UpdateId = messageId,
                        Message = new BotApiMessage
                        {
                            MessageId = messageId,
                            From = new BotApiUser
                            {
                                Id = senderUserId,
                                IsBot = false,
                                FirstName = "User"
                            },
                            Chat = new BotApiChat
                            {
                                Id = senderUserId, // в личных чатах ID чата совпадает с ID отправителя
                                Type = "private"
                            },
                            Date = date,
                            Text = text
                        }
                    };
                    
                    await updatesManager.AddUpdateAsync((int)toPeerId, update);
                    _processedMessageIds.Add(messageId);
                    
                    logger.LogInformation("Added update for bot {BotId}: MessageId={MessageId}, Text={Text}", toPeerId, messageId, text);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error polling for bot messages");
            }
            
            await Task.Delay(_pollInterval, stoppingToken);
        }
        
        logger.LogInformation("Bot Message Poller Service stopped");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Bot Message Poller Service crashed on startup");
            throw;
        }
    }
}
