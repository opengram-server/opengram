namespace MyTelegram.AuthServer.BackgroundServices;

public class MyTelegramAuthServerBackgroundService(
    ILogger<MyTelegramAuthServerBackgroundService> logger,
    IHandlerHelper handlerHelper,
    IFingerprintHelper fingerprintHelper
) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        handlerHelper.InitAllHandlers();
        logger.LogInformation("MyTelegram auth server started");
        const long defaultFingerprint = -3591632762792723036;
        var fingerprint = fingerprintHelper.GetFingerprint();
        if (fingerprint == defaultFingerprint)
        {
            logger.LogWarning("You are currently using the default private key, which anyone can obtain from the mytelegram open source project. For security reasons, please use your own private key and replace the client's public key.");
        }

        return Task.CompletedTask;
    }
}