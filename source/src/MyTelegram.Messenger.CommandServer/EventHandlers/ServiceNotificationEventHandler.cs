using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Events;
using MyTelegram.Messenger.Services.Interfaces;
using MyTelegram.Schema;

namespace MyTelegram.Messenger.CommandServer.EventHandlers;

public class ServiceNotificationEventHandler(
    IServiceNotificationAppService serviceNotificationAppService,
    ILogger<ServiceNotificationEventHandler> logger)
    : IEventHandler<ServiceNotificationEvent>, ITransientDependency
{
    public async Task HandleEventAsync(ServiceNotificationEvent integrationEvent)
    {
        logger.LogInformation("Received ServiceNotificationEvent for user {UserId}: Type={Type}, Popup={Popup}",
            integrationEvent.UserId, integrationEvent.Type, integrationEvent.Popup);

        try
        {
            // Разбираем медиа, если оно передано
            IMessageMedia? media = null;
            if (!string.IsNullOrEmpty(integrationEvent.MediaUrl))
            {
                // Пока ограничиваемся простым превью веб-страницы.
                // В продакшене медиа стоит скачать и загрузить отдельно.
                media = new TMessageMediaWebPage
                {
                    Webpage = new TWebPage
                    {
                        Id = 0,
                        Url = integrationEvent.MediaUrl,
                        DisplayUrl = integrationEvent.MediaUrl,
                        Hash = 0,
                        Type = integrationEvent.MediaType ?? "photo",
                        SiteName = "Notification",
                        Title = "Service Notification",
                        Description = integrationEvent.Message,
                        Photo = new TPhotoEmpty { Id = 0 },
                        EmbedUrl = string.Empty,
                        EmbedType = string.Empty,
                        EmbedWidth = 0,
                        EmbedHeight = 0,
                        Duration = 0,
                        Author = string.Empty,
                        Document = new TDocumentEmpty { Id = 0 }
                    }
                };
            }

            // Отправляем уведомление
            await serviceNotificationAppService.SendServiceNotificationAsync(
                integrationEvent.UserId,
                integrationEvent.Type,
                integrationEvent.Message,
                integrationEvent.Popup,
                media,
                entities: null,
                integrationEvent.InvertMedia
            );

            logger.LogInformation("ServiceNotificationEvent processed successfully for user {UserId}",
                integrationEvent.UserId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process ServiceNotificationEvent for user {UserId}",
                integrationEvent.UserId);
            throw;
        }
    }
}
