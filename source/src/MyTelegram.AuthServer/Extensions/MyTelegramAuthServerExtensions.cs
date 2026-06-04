using MyTelegram.EventBus.Extensions;

namespace MyTelegram.AuthServer.Extensions;

public static class MyTelegramAuthServerExtensions
{
    public static IServiceCollection AddAuthServer(this IServiceCollection services)
    {
        services.RegisterServices();

        services.AddMyTelegramHandlerServices();

        services.AddEventHandlers();

        return services;
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
        services.AddSubscription<UnencryptedMessage, UnencryptedMessageHandler>();
    }
}