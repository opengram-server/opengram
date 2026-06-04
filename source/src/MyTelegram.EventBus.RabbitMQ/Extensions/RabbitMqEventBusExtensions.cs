using Microsoft.Extensions.Hosting;

namespace MyTelegram.EventBus.RabbitMQ.Extensions;

public static class RabbitMqEventBusExtensions
{
    public static IServiceCollection AddMyTelegramRabbitMqEventBus(this IServiceCollection services)
    {
        services.AddTransient<IRabbitMqSerializer, RabbitMqSerializer>();
        services.AddSingleton<IEventBus, RabbitMqEventBus>();
        services.AddSingleton<IHostedService>(sp => (RabbitMqEventBus)sp.GetRequiredService<IEventBus>());

        return services;
    }
}
