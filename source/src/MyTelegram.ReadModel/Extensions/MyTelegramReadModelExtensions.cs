using Microsoft.Extensions.DependencyInjection;

namespace MyTelegram.ReadModel.Extensions;
public static class MyTelegramReadModelExtensions
{
    public static void AddMyTelegramReadModel(this IServiceCollection services)
    {
        services.RegisterServices(typeof(MyTelegramReadModelExtensions).Assembly);
    }
}
