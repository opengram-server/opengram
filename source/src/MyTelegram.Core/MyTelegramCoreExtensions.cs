using Microsoft.Extensions.DependencyInjection;

namespace MyTelegram.Core;

public static class MyTelegramCoreExtensions
{
    public static IServiceCollection AddMyTelegramCoreServices(this IServiceCollection services)
    {
        services.RegisterServices(typeof(MyTelegramCoreExtensions).Assembly);
        services.AddSingleton<IObjectMapper, DefaultObjectMapper>();
        services.AddSingleton(typeof(IObjectMapper<>), typeof(DefaultObjectMapper<>));
        services.AddSingleton(typeof(IMessageQueueProcessor<>), typeof(MessageQueueProcessor<>));

        return services;
    }
}