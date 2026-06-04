using MyTelegram.Domain.Shared.BotApi;

namespace MyTelegram.BotApi.Services;

public interface IUpdatesManager
{
    Task<List<BotApiUpdate>> GetUpdatesAsync(string token, int offset, int limit, int timeout, List<string>? allowedUpdates);
    Task AddUpdateAsync(long botUserId, BotApiUpdate update);
    Task ClearUpdatesAsync(string token, long offset);
    Task StoreInvoiceAsync(string payload, object invoiceData);
    Task AnswerPreCheckoutAsync(string preCheckoutQueryId, bool ok, string? errorMessage);
}
