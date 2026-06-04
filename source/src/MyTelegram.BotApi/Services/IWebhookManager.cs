using MyTelegram.Domain.Shared.BotApi;

namespace MyTelegram.BotApi.Services;

public interface IWebhookManager
{
    Task SetWebhookAsync(string token, string url, IFormFile? certificate, string? ipAddress, 
        int? maxConnections, string[]? allowedUpdates, bool? dropPendingUpdates, string? secretToken);
    Task DeleteWebhookAsync(string token, bool? dropPendingUpdates);
    Task<BotApiWebhookInfo> GetWebhookInfoAsync(string token);
    Task SendUpdateToWebhookAsync(long botUserId, object update);
}
