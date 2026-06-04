using Microsoft.Extensions.Hosting;
using MyTelegram.Messenger.Services.Caching;

namespace MyTelegram.Messenger.CommandServer.BackgroundServices;

public class MyTelegramCommandServerBackgroundService(
    ILogger<MyTelegramCommandServerBackgroundService> logger,
    IHandlerHelper handlerHelper,
    IInMemoryCacheLoader inMemoryCacheLoader)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Command server starting...");
        handlerHelper.InitAllHandlers();
        await inMemoryCacheLoader.LoadAsync();

        logger.LogInformation("Command server started");
    }
}
