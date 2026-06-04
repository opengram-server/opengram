using Microsoft.Extensions.DependencyInjection;
using EventFlow.Queries;
using MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;
using MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;
using MyTelegram.Messenger.Handlers.LatestLayer.Impl.Channels;
using MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.EventFlow.MongoDB.ReadStores;
using MyTelegram.EventFlow.ReadStores;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.Messenger.Extensions;

/// <summary>
/// Extension method to register all new API handlers
/// </summary>
public static class MyTelegramHandlerServicesExtensions
{
    /// <summary>
    /// Adds all new Telegram API handlers to the service collection
    /// </summary>
    public static IServiceCollection AddMyTelegramNewHandlerServices(this IServiceCollection services)
    {
        // Register Affiliate Program handlers
        services.AddScoped<IUpdateStarRefProgramHandler, UpdateStarRefProgramHandler>();
        
        // Register Gigagroup handlers
        services.AddScoped<IConvertToGigagroupHandler, ConvertToGigagroupHandler>();
        
        // Register Business handlers (if not already registered)
        services.AddScoped<ICreateBusinessChatLinkHandler, CreateBusinessChatLinkHandler>();
        
        // Register Monoforum handlers
        services.AddScoped<ICreateMonoforumHandler, CreateMonoforumHandler>();
        
        // Register Checklist handlers
        services.AddScoped<ICreateChecklistHandler, CreateChecklistHandler>();
        
        // Register Direct Message handlers
        services.AddScoped<ICreateDirectMessageHandler, CreateDirectMessageHandler>();
        
        // Register Suggested Post handlers
        services.AddScoped<ICreateSuggestedPostHandler, CreateSuggestedPostHandler>();
        
        // Register Stars Rating handlers
        services.AddScoped<IUpdateStarsRatingHandler, UpdateStarsRatingHandler>();

        // Register Star Gifts handlers
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments.IGetStarGiftsHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments.GetStarGiftsHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Payments.IGetStarGiftHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments.GetStarGiftHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments.IGetSavedStarGiftsHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments.GetSavedStarGiftsHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Payments.ICreateStarGiftCollectionHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments.CreateStarGiftCollectionHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Payments.ICheckCanSendGiftHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments.CheckCanSendGiftHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Payments.IGetUserStarGiftsHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments.GetUserStarGiftsHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Payments.ISaveStarGiftHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments.SaveStarGiftHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Payments.IConvertStarGiftHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments.ConvertStarGiftHandler>();

        // Register Quick Reply handlers
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages.IGetQuickRepliesHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages.GetQuickRepliesHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages.ISendQuickReplyMessagesHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages.SendQuickReplyMessagesHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages.IEditQuickReplyShortcutHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages.EditQuickReplyShortcutHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages.IDeleteQuickReplyShortcutHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages.DeleteQuickReplyShortcutHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages.IReorderQuickRepliesHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages.ReorderQuickRepliesHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages.ICheckQuickReplyShortcutHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages.CheckQuickReplyShortcutHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages.IDeleteQuickReplyMessagesHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages.DeleteQuickReplyMessagesHandler>();
        services.AddScoped<MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Messages.IGetQuickReplyMessagesHandler, MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages.GetQuickReplyMessagesHandler>();

        // Register Monoforum Query Handler
        services.AddTransient<IQueryOnlyReadModelStore<ChannelReadModel>, MongoDbQueryOnlyReadModelStore<ChannelReadModel>>();
        services.AddTransient<IQueryHandler<GetMonoforumQuery, Monoforum>, GetMonoforumQueryHandler>();

        return services;
    }
}
