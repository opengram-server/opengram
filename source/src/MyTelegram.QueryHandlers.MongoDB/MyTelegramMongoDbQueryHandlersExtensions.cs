namespace MyTelegram.QueryHandlers.MongoDB;

public static class MyTelegramMongoDbQueryHandlersExtensions
{
    public static IEventFlowOptions AddMongoDbQueryHandlers(this IEventFlowOptions options)
    {
        return options.AddQueryHandlers(typeof(MyTelegramMongoDbQueryHandlersExtensions).Assembly);
    }
}
