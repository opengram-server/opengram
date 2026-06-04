namespace MyTelegram.MTProto.Extensions;
public static class MyTelegramMtProtoExtensions
{
    public static void AddMyTelegramMtProto(this IServiceCollection services)
    {
        services.RegisterServices(typeof(MyTelegramMtProtoExtensions).Assembly);
    }
}
