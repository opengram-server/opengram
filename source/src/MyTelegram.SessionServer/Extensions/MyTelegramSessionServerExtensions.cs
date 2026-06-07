using MyTelegram.Core;
using MyTelegram.EventBus;
using MyTelegram.EventBus.Extensions;
using MyTelegram.SessionServer.BackgroundServices;
using MyTelegram.SessionServer.Caching;
using MyTelegram.SessionServer.EventHandlers;
using MyTelegram.SessionServer.Handlers;
using MyTelegram.SessionServer.Handlers.Impl;
using MyTelegram.SessionServer.Handlers.Impl.Auth;
using MyTelegram.SessionServer.Options;
using MyTelegram.SessionServer.Services;
using MyTelegram.SessionServer.Services.Impl;

namespace MyTelegram.SessionServer.Extensions;

/// <summary>
/// DI registration for the new SessionServer architecture.
/// Reconstructed from the original binary's AddMyTelegramHandlerServices / AddMyEventFlow.
/// </summary>
public static class MyTelegramSessionServerExtensions
{
    public static IServiceCollection AddMyTelegramSessionServer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Options
        services.Configure<MyTelegramSessionServerOptions>(
            configuration.GetSection("TelegramServerSettings"));

        // Caching (all singletons — ConcurrentDictionary-based, as in the original)
        services.AddSingleton<IAuthKeyHelper, AuthKeyHelper>();
        services.AddSingleton<IOnlineUserHelper, OnlineUserHelper>();
        services.AddSingleton<IChatMemberHelper, ChatMemberHelper>();
        services.AddSingleton<IRequestCacheAppService, RequestCacheAppService>();
        services.AddSingleton<IPendingRequestTracker, PendingRequestTracker>();

        // Core services (singletons for in-memory state)
        services.AddSingleton<ISessionService, SessionService>();
        services.AddSingleton<IServerSaltHelper, ServerSaltHelper>();
        services.AddSingleton<PermissionChecker>();
        services.AddSingleton<ISessionDataProcessor, SessionDataProcessor>();

        // Scoped/transient services
        services.AddSingleton<ISessionDataDispatcher, SessionDataDispatcher>();
        services.AddSingleton<IMessageSender2, MessageSender2>();
        services.AddSingleton<IEncryptedMessageProcessor, EncryptedMessageProcessor>();

        // Handlers
        services.AddTransient<GetFutureSaltsHandler>();
        services.AddTransient<DestroySessionHandler>();
        services.AddTransient<DestroyAuthKeyHandler>();
        services.AddTransient<InvokeWithLayerHandler>();
        services.AddTransient<InitConnectionHandler>();
        services.AddTransient<InvokeAfterMsgHandler>();
        services.AddTransient<InvokeWithoutUpdatesHandler>();
        services.AddTransient<MsgsStateReqHandler>();
        services.AddTransient<GzipPackedHandler>();
        services.AddTransient<BindTempAuthKeyHandler>();
        services.AddTransient<LogOutHandler>();
        services.AddTransient<DropTempAuthKeysHandler>();
        services.AddTransient<ResetAuthorizationsHandler>();

        // Event handlers
        services.AddTransient<SessionEventHandler>();

        // Event subscriptions (16 event types → SessionEventHandler)
        services.AddSubscription<AuthKeyCreatedIntegrationEvent, SessionEventHandler>();
        services.AddSubscription<BindUserIdToAuthKeyIntegrationEvent, SessionEventHandler>();
        services.AddSubscription<BindUserIdToAuthKeySuccessEvent, SessionEventHandler>();
        services.AddSubscription<BindUserIdToSessionEvent, SessionEventHandler>();
        services.AddSubscription<ClientDisconnectedEvent, SessionEventHandler>();
        services.AddSubscription<DeviceRegisteredEvent, SessionEventHandler>();
        services.AddSubscription<SessionPasswordStateChangedEvent, SessionEventHandler>();
        services.AddSubscription<SetSessionPasswordStateEvent, SessionEventHandler>();
        services.AddSubscription<UserSignInSuccessEvent, SessionEventHandler>();
        services.AddSubscription<UnRegisterAuthKeyEvent, SessionEventHandler>();
        services.AddSubscription<SessionRevokedEvent, SessionEventHandler>();
        services.AddSubscription<AuthKeyNotFoundEvent, SessionEventHandler>();
        services.AddSubscription<DataResultResponseReceivedEvent, SessionEventHandler>();
        services.AddSubscription<LayeredPushMessageCreatedIntegrationEvent, SessionEventHandler>();
        services.AddSubscription<ChatMemberChangedEvent, SessionEventHandler>();
        services.AddSubscription<ChannelMemberChangedEvent, SessionEventHandler>();

        // Background services
        services.AddHostedService<SessionDataProcessorBackgroundService>();
        services.AddHostedService<RemoveExpiredAuthKeysBackgroundService>();
        services.AddHostedService<MongoDbIndexesCreatorBackgroundService>();
        services.AddHostedService<ObjectMessageSenderBackgroundService>();
        services.AddHostedService<QueuedCommandExecutorBackgroundService>();
        services.AddHostedService<LayeredServiceSubscribeBackgroundService>();

        return services;
    }
}
