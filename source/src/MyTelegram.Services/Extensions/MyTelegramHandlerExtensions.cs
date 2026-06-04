using Microsoft.Extensions.DependencyInjection;
using MyTelegram.Services.NativeAot;
using MyTelegram.Services.Services;
namespace MyTelegram.Services.Extensions
{
    public static class MyTelegramHandlerExtensions
    {
        public static IServiceCollection AddMyTelegramHandlerServices(this IServiceCollection services)
        {
            services.AddMyTelegramCoreServices();
            services.AddMyEventFlow();

            services.RegisterServices(typeof(MyTelegramHandlerExtensions).Assembly);
            services.AddSingleton(typeof(IInMemoryRepository<,>), typeof(InMemoryRepository<,>));
            services.AddTransient(typeof(IDataProcessor<>), typeof(DefaultDataProcessor<>));
            services.AddSingleton(typeof(IMessageQueueProcessor<>), typeof(MessageQueueProcessor<>));
            services.AddTransient<IDataProcessor<ISessionMessage>, SessionMessageDataProcessor>();
            services.AddSingleton(typeof(IReadModelCacheHelper<>), typeof(ReadModelCacheHelper<>));

            services.AddSystemTextJson(options =>
            {
                options.TypeInfoResolverChain.Add(MyJsonSerializeContext.Default);
            });

            services.AddSingleton(typeof(ILayeredService<>), typeof(LayeredService<>));
            services.AddSingleton(typeof(ICacheHelper<,>), typeof(CacheHelper<,>));
            services.AddSingleton(typeof(IQueuedCommandExecutor<,,>), typeof(QueuedCommandExecutor<,,>));

            return services;
        }
    }
}
