using MyTelegram.Domain.Shared.BotApi;

namespace MyTelegram.BotApi.Services;

public interface IUpdatesManager
{
    Task<List<BotApiUpdate>> GetUpdatesAsync(string token, int offset, int limit, int timeout, List<string>? allowedUpdates);
    Task AddUpdateAsync(long botUserId, BotApiUpdate update);
    Task ClearUpdatesAsync(string token, long offset);
    Task StoreInvoiceAsync(string payload, object invoiceData);
    Task AnswerPreCheckoutAsync(string preCheckoutQueryId, bool ok, string? errorMessage);
    Task SetWebhookAsync(string token, string url, string? secretToken, int? maxConnections, List<string>? allowedUpdates);
    Task DeleteWebhookAsync(string token);
    Task<WebhookInfo?> GetWebhookInfoAsync(string token);
}

public class WebhookInfo
{
    public string Url { get; set; } = string.Empty;
    public bool HasCustomCertificate { get; set; }
    public int PendingUpdateCount { get; set; }
    public string? IpAddress { get; set; }
    public long? LastErrorDate { get; set; }
    public string? LastErrorMessage { get; set; }
    public int? MaxConnections { get; set; }
    public List<string>? AllowedUpdates { get; set; }
}
