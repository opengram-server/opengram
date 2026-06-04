using EventFlow.Core.Caching;
using EventFlow.MongoDB.Extensions;
using MyTelegram.Caching.Redis;
using MyTelegram.Converters.Extensions;
using MyTelegram.Messenger.NativeAot;
using MyTelegram.QueryHandlers.MongoDB;
using MyTelegram.ReadModel.MongoDB;
using MyTelegram.ReadModel.ReadModelLocators;
using MyTelegram.Services.NativeAot;
using ChannelFullReadModel = MyTelegram.ReadModel.MongoDB.ChannelFullReadModel;
using ChannelReadModel = MyTelegram.ReadModel.MongoDB.ChannelReadModel;
using PhotoReadModel = MyTelegram.ReadModel.MongoDB.PhotoReadModel;
using PtsForAuthKeyIdReadModel = MyTelegram.ReadModel.MongoDB.PtsForAuthKeyIdReadModel;
using PtsReadModel = MyTelegram.ReadModel.MongoDB.PtsReadModel;
using UserReadModel = MyTelegram.ReadModel.MongoDB.UserReadModel;

namespace MyTelegram.Messenger.Extensions;

public static class MyTelegramMessengerServerExtensions
{
    public static IServiceCollection AddMyTelegramMessengerServices(this IServiceCollection services)
    {
        services.RegisterMongoDbSerializer();
        services.RegisterServices();
        services.AddMyTelegramHandlerServices();
        services.AddMyEventFlow();
        services.AddMyTelegramConverters();
        
        // Register new Telegram API services
        services.AddMyTelegramNewServices();
        
        // Register new API handler services
        services.AddMyTelegramNewHandlerServices();

        services.AddTransient<IChatInviteLinkHelper, ChatInviteLinkHelper>();
        services.AddTransient<MyTelegram.Domain.Services.IDiffieHellmanService, MyTelegram.Domain.Services.Impl.DiffieHellmanService>();
        services.AddSingleton(typeof(IDomainEventCacheHelper<>), typeof(DomainEventCacheHelper<>));
        //services.AddSingleton(typeof(IReadModelCacheHelper<>), typeof(ReadModelCacheHelper<>));
        services.AddSingleton(typeof(IInMemoryRepository<,>), typeof(InMemoryRepository<,>));

        services.AddTransient<ICachedReadModelManager, MultipleAggregateCachedReadModelManager<IUserReadModel, UserReadModel, IUserReadModelLocator>>();
        services.AddTransient<ICachedReadModelManager, MultipleAggregateCachedReadModelManager<IChannelReadModel, ChannelReadModel, IChannelReadModelLocator>>();
        services.AddTransient<ICachedReadModelManager, SingleAggregateCachedReadModelManager<IPhotoReadModel, PhotoReadModel>>();
        services.AddTransient<ICachedReadModelManager, MultipleAggregateCachedReadModelManager<IChannelFullReadModel, ChannelFullReadModel, IChannelFullReadModelLocator>>();
        services.AddTransient<ICachedReadModelManager, SingleAggregateCachedReadModelManager<IPtsReadModel, PtsReadModel>>();
        services.AddTransient<ICachedReadModelManager, SingleAggregateCachedReadModelManager<IPtsForAuthKeyIdReadModel, PtsForAuthKeyIdReadModel>>();

        services.AddCacheJsonSerializer(jsonOptions =>
        {
            jsonOptions.TypeInfoResolverChain.Add(MyJsonSerializeContext.Default);
            jsonOptions.TypeInfoResolverChain.Add(MyMessengerJsonContext.Default);
        });

        services.AddSystemTextJson(options =>
        {
            options.TypeInfoResolverChain.Add(MyMessengerJsonContext.Default);
        });

        return services;
    }
}