using MyTelegram.EventBus.Extensions;

namespace MyTelegram.SmsSender.Extensions;
public static class Extensions
{
    public static IServiceCollection AddMyTelegramSmsSender(this IServiceCollection services)
    {
        services.RegisterServices();
        services.AddEventHandlers();

        return services;
    }

    public static void AddEventHandlers(this IServiceCollection services)
    {
        services.AddSubscription<AppCodeCreatedIntegrationEvent, AppCodeEventHandler>();
    }

    public static Task SendAsync(this ISmsSender smsSender, string phoneNumber, string text)
    {
        return smsSender.SendAsync(new SmsMessage(phoneNumber, text));
    }
}
