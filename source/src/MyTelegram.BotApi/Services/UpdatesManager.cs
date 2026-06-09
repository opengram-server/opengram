using MyTelegram.Domain.Shared.BotApi;
using MyTelegram.ReadModel.Impl;
using System.Collections.Concurrent;
using System.Text.Json;
using MongoDB.Driver;

namespace MyTelegram.BotApi.Services;

public class UpdatesManager(
    IMongoDatabase database,
    ILogger<UpdatesManager> logger) : IUpdatesManager
{
    private readonly ConcurrentDictionary<long, ConcurrentQueue<BotApiUpdate>> _updatesQueues = new();
    private readonly ConcurrentDictionary<long, long> _lastUpdateIds = new();
    private readonly ConcurrentDictionary<string, object> _invoices = new();
    private readonly ConcurrentDictionary<string, (bool ok, string? error)> _preCheckoutAnswers = new();
    private readonly ConcurrentDictionary<string, WebhookConfig> _webhooks = new();

    private class WebhookConfig
    {
        public string Url { get; set; } = string.Empty;
        public string? SecretToken { get; set; }
        public int? MaxConnections { get; set; }
        public List<string>? AllowedUpdates { get; set; }
        public DateTime SetAt { get; set; }
    }

    public async Task<List<BotApiUpdate>> GetUpdatesAsync(string token, int offset, int limit, int timeout, List<string>? allowedUpdates)
    {
        // Получаем бота напрямую из MongoDB по токену
        var botsCollection = database.GetCollection<BotReadModel>("ReadModel-BotReadModel");
        var bot = await botsCollection.Find(b => b.Token == token).FirstOrDefaultAsync();
        
        if (bot == null)
        {
            throw new Exception("Invalid bot token");
        }

        if (!_updatesQueues.TryGetValue(bot.UserId, out var queue))
        {
            queue = new ConcurrentQueue<BotApiUpdate>();
            _updatesQueues[bot.UserId] = queue;
        }

        logger.LogInformation("Getting updates for bot {BotId}, queue size: {QueueSize}, offset: {Offset}", bot.UserId, queue.Count, offset);

        var updates = new List<BotApiUpdate>();
        var startTime = DateTime.UtcNow;

        while (updates.Count < limit && (DateTime.UtcNow - startTime).TotalSeconds < timeout)
        {
            if (queue.TryPeek(out var update))
            {
                if (update.UpdateId >= offset)  // >= чтобы включить update с указанным offset
                {
                    // Извлекаем из очереди только то, что действительно используем
                    queue.TryDequeue(out update);

                    if (allowedUpdates == null || allowedUpdates.Count == 0 ||
                        allowedUpdates.Contains(BotApiConverter.GetUpdateType(update)))
                    {
                        updates.Add(update);
                    }
                }
                else
                {
                    // Старые обновления (UpdateId < offset) отбрасываем
                    queue.TryDequeue(out _);
                }
            }
            else if (timeout > 0)
            {
                await Task.Delay(50); // короткая пауза, чтобы отвечать почти мгновенно
            }
            else
            {
                break;
            }
        }

        logger.LogInformation("Bot {BotId} received {Count} updates (offset={Offset})", bot.UserId, updates.Count, offset);
        return updates;
    }

    public Task AddUpdateAsync(long botUserId, BotApiUpdate update)
    {
        if (!_updatesQueues.TryGetValue(botUserId, out var queue))
        {
            queue = new ConcurrentQueue<BotApiUpdate>();
            _updatesQueues[botUserId] = queue;
        }

        // Назначаем обновлению следующий по порядку UpdateId для этого бота
        var lastUpdateId = _lastUpdateIds.GetOrAdd(botUserId, 0);
        var newUpdateId = Interlocked.Increment(ref lastUpdateId);
        _lastUpdateIds[botUserId] = newUpdateId;
        
        update.UpdateId = newUpdateId;

        queue.Enqueue(update);
        logger.LogInformation("Added update {UpdateId} for bot {BotId}, queue size: {QueueSize}", update.UpdateId, botUserId, queue.Count);
        return Task.CompletedTask;
    }

    public void AddUpdate(long botUserId, object updateData)
    {
        // Преобразуем анонимный объект в BotApiUpdate через JSON
        var json = JsonSerializer.Serialize(updateData);
        var update = JsonSerializer.Deserialize<BotApiUpdate>(json);
        
        if (update != null)
        {
            AddUpdateAsync(botUserId, update).Wait();
        }
    }

    public async Task ClearUpdatesAsync(string token, long offset)
    {
        // Получаем бота напрямую из MongoDB по токену
        var botsCollection = database.GetCollection<MyTelegram.ReadModel.Impl.BotReadModel>("ReadModel-BotReadModel");
        var bot = await botsCollection.Find(b => b.Token == token).FirstOrDefaultAsync();
        
        if (bot == null)
        {
            throw new Exception("Invalid bot token");
        }

        if (_updatesQueues.TryGetValue(bot.UserId, out var queue))
        {
            var newQueue = new ConcurrentQueue<BotApiUpdate>();
            while (queue.TryDequeue(out var update))
            {
                if (update.UpdateId > offset)
                {
                    newQueue.Enqueue(update);
                }
            }
            _updatesQueues[bot.UserId] = newQueue;
        }
    }

    public Task StoreInvoiceAsync(string payload, object invoiceData)
    {
        _invoices[payload] = invoiceData;
        logger.LogInformation("Stored invoice with payload: {Payload}", payload);
        return Task.CompletedTask;
    }

    public Task AnswerPreCheckoutAsync(string preCheckoutQueryId, bool ok, string? errorMessage)
    {
        _preCheckoutAnswers[preCheckoutQueryId] = (ok, errorMessage);
        logger.LogInformation("Pre-checkout answer stored for query {QueryId}: ok={Ok}", preCheckoutQueryId, ok);
        return Task.CompletedTask;
    }

    public Task SetWebhookAsync(string token, string url, string? secretToken, int? maxConnections, List<string>? allowedUpdates)
    {
        _webhooks[token] = new WebhookConfig
        {
            Url = url,
            SecretToken = secretToken,
            MaxConnections = maxConnections,
            AllowedUpdates = allowedUpdates,
            SetAt = DateTime.UtcNow
        };
        logger.LogInformation("Webhook set for token {Token}: {Url}", token[..8] + "...", url);
        return Task.CompletedTask;
    }

    public Task DeleteWebhookAsync(string token)
    {
        _webhooks.TryRemove(token, out _);
        logger.LogInformation("Webhook deleted for token {Token}", token[..8] + "...");
        return Task.CompletedTask;
    }

    public Task<WebhookInfo?> GetWebhookInfoAsync(string token)
    {
        if (_webhooks.TryGetValue(token, out var config))
        {
            return Task.FromResult<WebhookInfo?>(new WebhookInfo
            {
                Url = config.Url,
                HasCustomCertificate = false,
                PendingUpdateCount = 0,
                MaxConnections = config.MaxConnections,
                AllowedUpdates = config.AllowedUpdates
            });
        }

        return Task.FromResult<WebhookInfo?>(new WebhookInfo
        {
            Url = string.Empty,
            HasCustomCertificate = false,
            PendingUpdateCount = 0
        });
    }
}
