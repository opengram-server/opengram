using EventFlow;
using EventFlow.Extensions;
using MyTelegram.Domain.Aggregates.Channel;
using MyTelegram.Domain.Aggregates.Chat;
using MyTelegram.Domain.EventFlow;
using MyTelegram.EventFlow.MongoDB;
using MyTelegram.QueryHandlers.MongoDB.Channel;
using MyTelegram.QueryHandlers.MongoDB.Chat;
using MyTelegram.ReadModel.MongoDB;

namespace MyTelegram.MessengerServer.GrpcService;

public static class MyTelegramMessengerServerGrpcServiceExtensions
{
    private static IServiceCollection AddMyTelegramGrpcService(this IServiceCollection services)
    {
        return services;
    }

    public static void UseMyTelegramMessengerGrpcServer(this IServiceCollection services,
        Action<IEventFlowOptions>? configure = null)
    {
        services.AddEventFlow(options =>
            {
                options.UseMongoDbReadModel<ChatAggregate, ChatId, ChatReadModel>();
                options.UseMongoDbReadModel<ChannelMemberAggregate, ChannelMemberId, ChannelMemberReadModel>();

                options.AddQueryHandlers(
                    typeof(GetChatByChatIdQueryHandler),
                    typeof(GetChannelMembersByChannelIdQueryHandler)
                );
                options.AddMyMongoDbReadModel();

                configure?.Invoke(options);
            })
            //.AddMyTelegramCoreServices()
            //.AddMyTelegramHandlerServices()
            //.AddMyTelegramMessengerServer()
            .AddMyTelegramGrpcService()
            .AddMyEventFlow()
            //.RegisterAllMappers()
            ;
    }
}
