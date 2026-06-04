using Microsoft.Extensions.DependencyInjection;
using MyTelegram.Messenger.Services;
using MyTelegram.Messenger.Services.Impl;

namespace MyTelegram.Messenger;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGroupCallServices(this IServiceCollection services)
    {
        // Register group call services
        services.AddSingleton<IIceCredentialsService, IceCredentialsService>();
        services.AddScoped<ISsrcManagementService, SsrcManagementService>();
        services.AddScoped<IMediasoupBridgeService, MediasoupBridgeService>();
        services.AddScoped<IGroupCallConnectionService, GroupCallConnectionService>();
        services.AddSingleton<IFrozenSettingsService, FrozenSettingsService>();

        // Configure HTTP client for Mediasoup API
        services.AddHttpClient("Mediasoup", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        return services;
    }
}
