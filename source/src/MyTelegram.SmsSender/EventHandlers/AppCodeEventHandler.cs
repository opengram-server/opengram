namespace MyTelegram.SmsSender.EventHandlers;

public class AppCodeEventHandler(ISmsSenderFactory smsSenderFactory, ILogger<AppCodeEventHandler> logger)
    : IEventHandler<AppCodeCreatedIntegrationEvent>, ITransientDependency
{
    public async Task HandleEventAsync(AppCodeCreatedIntegrationEvent eventData)
    {
        var phoneNumber = eventData.PhoneNumber;
        if (!phoneNumber.StartsWith("+"))
        {
            phoneNumber = $"+{phoneNumber}";
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (eventData.Expire < now)
        {
            logger.LogWarning("App code expired, data={@Data}", eventData);
            return;
        }

        try
        {
            var smsSender = smsSenderFactory.Create(eventData.PhoneNumber);
            await smsSender.SendAsync(phoneNumber, $"MyTelegram code: {eventData.Code}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Send sms failed, data={@Data}", eventData);
        }
    }
}
