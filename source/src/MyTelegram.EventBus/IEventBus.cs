namespace MyTelegram.EventBus;

public interface IEventBus
{
    Task PublishAsync<TEventData>(TEventData eventData, string? eventType = null) where TEventData : class;
}