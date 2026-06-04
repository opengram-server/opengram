using Microsoft.Extensions.DependencyInjection;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Messenger.Services.Interfaces;

namespace MyTelegram.Messenger.Extensions;

/// <summary>
/// Extension method to register all new Telegram API services
/// </summary>
public static class MyTelegramNewServicesExtensions
{
    /// <summary>
    /// Adds all new Telegram API services to the service collection
    /// </summary>
    public static IServiceCollection AddMyTelegramNewServices(this IServiceCollection services)
    {
        // Register Affiliate Program services
        services.AddScoped<IAffiliateAppService, AffiliateAppService>();
        
        // Register Gigagroup services  
        services.AddScoped<IGigagroupAppService, GigagroupAppService>();
        
        // Register Monoforum services
        services.AddScoped<IMonoforumAppService, MonoforumAppService>();
        
        // Register Checklist services
        services.AddScoped<IChecklistAppService, ChecklistAppService>();
        
        // Register Direct Message services
        services.AddScoped<IDirectMessageAppService, DirectMessageAppService>();
        
        // Register Suggested Post services
        services.AddScoped<ISuggestedPostAppService, SuggestedPostAppService>();
        
        // Register Stars Rating services
        services.AddScoped<IStarsRatingAppService, StarsRatingAppService>();

        // Business service is now enabled
        services.AddScoped<IBusinessAppService, BusinessAppService>();
        
        // Privacy service is now enabled
        services.AddScoped<IPrivacyAppService, PrivacyAppService>();
        
        // Stars service is now enabled
        services.AddScoped<IStarsAppService, StarsAppService>();
        
        // Quick Replies service is now enabled
        services.AddScoped<IQuickRepliesAppService, QuickRepliesAppService>();
        
        // Sticker service for business intro and other sticker operations
        services.AddScoped<IStickerAppService, StickerAppService>();
        
        // Paid Messages service is now enabled
        services.AddScoped<IPaidMessagesAppService, PaidMessagesAppService>();
        
        // Privacy Helper
        services.AddScoped<IPrivacyHelper, PrivacyHelper>();

        // Star Gift Attribute Generator
        services.AddScoped<MyTelegram.Messenger.Services.Interfaces.IStarGiftAttributeGenerator, StarGiftAttributeGenerator>();

        return services;
    }
}
