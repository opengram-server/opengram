namespace MyTelegram.EventBus;

public interface IEventHandler<in TEvent>
    where TEvent : class
{
    Task HandleEventAsync(TEvent eventData);
}