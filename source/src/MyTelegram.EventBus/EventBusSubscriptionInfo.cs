using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace MyTelegram.EventBus;

public class EventBusSubscriptionInfo
{
    public Dictionary<string, Type> EventTypes { get; } = [];
    public HashSet<string> RawDataHandledEventTypes { get; } = [];

    private readonly ConcurrentDictionary<Type, List<Func<object, IServiceProvider, Task>>> _handlers = new();
    public void Register<TEvent>() where TEvent : class
    {
        var eventType = typeof(TEvent);
        var handler = new Func<object, IServiceProvider, Task>((obj, sp) =>
        {
            var handler = sp.GetRequiredKeyedService<IEventHandler<TEvent>>(typeof(TEvent));

            return handler.HandleEventAsync((TEvent)obj);
        });
        _handlers.AddOrUpdate(eventType, _ => [handler], (_, list) =>
        {
            list.Add(handler);
            return list;
        });
    }

    public bool TryGetHandlers(Type eventType, out List<Func<object, IServiceProvider, Task>>? handlers)
    {
        return _handlers.TryGetValue(eventType, out handlers);
    }
}