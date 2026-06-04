using MyTelegram.Domain.Shared.BotApi;
using MyTelegram.ReadModel.Impl;
using System.Text;
using System.Text.Json;
using MongoDB.Driver;

namespace MyTelegram.BotApi.Services;

public class WebhookManager(
    IMongoDatabase database,
    IHttpClientFactory httpClientFactory,
    ILogger<WebhookManager> logger) : IWebhookManager
{
    private readonly Dictionary<long, WebhookConfig> _webhooks = new();

    public async Task SetWebhookAsync(string token, string url, IFormFile? certificate, string? ipAddress,
        int? maxConnections, string[]? allowedUpdates, bool? dropPendingUpdates, string? secretToken)
    {
        // Query bot directly from MongoDB
        var botsCollection = database.GetCollection<BotReadModel>("ReadModel-BotReadModel");
        var bot = await botsCollection.Find(b => b.Token == token).FirstOrDefaultAsync();
        
        if (bot == null)
        {
            throw new Exception("Invalid bot token");
        }

        _webhooks[bot.UserId] = new WebhookConfig
        {
            Url = url,
            IpAddress = ipAddress,
            MaxConnections = maxConnections ?? 40,
            AllowedUpdates = allowedUpdates,
            SecretToken = secretToken
        };

        logger.LogInformation("Webhook set for bot {BotId}: {Url}", bot.UserId, url);
    }

    public async Task DeleteWebhookAsync(string token, bool? dropPendingUpdates)
    {
        // Query bot directly from MongoDB
        var botsCollection = database.GetCollection<BotReadModel>("ReadModel-BotReadModel");
        var bot = await botsCollection.Find(b => b.Token == token).FirstOrDefaultAsync();
        
        if (bot == null)
        {
            throw new Exception("Invalid bot token");
        }

        _webhooks.Remove(bot.UserId);
        logger.LogInformation("Webhook deleted for bot {BotId}", bot.UserId);
    }

    public async Task<BotApiWebhookInfo> GetWebhookInfoAsync(string token)
    {
        // Query bot directly from MongoDB
        var botsCollection = database.GetCollection<MyTelegram.ReadModel.Impl.BotReadModel>("ReadModel-BotReadModel");
        var bot = await botsCollection.Find(b => b.Token == token).FirstOrDefaultAsync();
        
        if (bot == null)
        {
            throw new Exception("Invalid bot token");
        }

        if (_webhooks.TryGetValue(bot.UserId, out var config))
        {
            return new BotApiWebhookInfo
            {
                Url = config.Url,
                HasCustomCertificate = false,
                PendingUpdateCount = 0,
                IpAddress = config.IpAddress,
                MaxConnections = config.MaxConnections,
                AllowedUpdates = config.AllowedUpdates?.ToList()
            };
        }

        return new BotApiWebhookInfo
        {
            Url = "",
            HasCustomCertificate = false,
            PendingUpdateCount = 0
        };
    }

    public async Task SendUpdateToWebhookAsync(long botUserId, object update)
    {
        if (!_webhooks.TryGetValue(botUserId, out var config))
        {
            return;
        }

        try
        {
            var client = httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(update);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            if (!string.IsNullOrEmpty(config.SecretToken))
            {
                content.Headers.Add("X-Telegram-Bot-Api-Secret-Token", config.SecretToken);
            }

            var response = await client.PostAsync(config.Url, content);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Webhook delivery failed for bot {BotId}: {StatusCode}", 
                    botUserId, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending webhook for bot {BotId}", botUserId);
        }
    }

    private class WebhookConfig
    {
        public string Url { get; set; } = default!;
        public string? IpAddress { get; set; }
        public int MaxConnections { get; set; }
        public string[]? AllowedUpdates { get; set; }
        public string? SecretToken { get; set; }
    }
}
