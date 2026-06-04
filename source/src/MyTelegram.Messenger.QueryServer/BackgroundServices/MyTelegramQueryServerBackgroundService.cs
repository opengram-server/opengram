using Microsoft.Extensions.Hosting;
using MyTelegram.Messenger.Services.Caching;

namespace MyTelegram.Messenger.QueryServer.BackgroundServices;

public class MyTelegramQueryServerBackgroundService(
    ILogger<MyTelegramQueryServerBackgroundService> logger,
    //IHandlerHelper handlerHelper,
    IInMemoryCacheLoader inMemoryCacheLoader,
    ILanguageCacheService languageCacheService)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Query server starting...");
        await inMemoryCacheLoader.LoadAsync();
        await languageCacheService.LoadAllLanguagesAsync();
        await languageCacheService.LoadAllLanguageTextAsync();

        logger.LogInformation("Query server started");
    }
}
