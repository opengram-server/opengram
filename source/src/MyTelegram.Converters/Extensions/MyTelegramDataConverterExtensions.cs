using Microsoft.Extensions.DependencyInjection;

namespace MyTelegram.Converters.Extensions;

public static class MyTelegramDataConverterExtensions
{
    public static void AddMyTelegramConverters(this IServiceCollection services)
    {
        var assembly = typeof(MyTelegramDataConverterExtensions).Assembly;
        services.RegisterServices(assembly);
    }
}