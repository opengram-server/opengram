//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Rebus.Bus;
//using Rebus.Transport;

//namespace MyTelegram.EventBus.Rebus;

//public class RebusEventBus(
//    IBus bus,
//    IEventBusSubscriptionsManager subsManager,
//    ILogger<RebusEventBus> logger,
//    IServiceProvider serviceProvider,
//    IEventHandlerInvoker eventHandlerInvoker)
//    : IEventBus
//{
//    public async Task PublishAsync<TEventData>(TEventData eventData) where TEventData : class
//    {
//        using var scope = new RebusTransactionScope();
//        {
//            await bus.Publish(eventData);
//            await scope.CompleteAsync();
//        }
//    }

//    public async Task PublishAsync(Type eventDataType, object eventData)
//    {
//        using var scope = new RebusTransactionScope();
//        {
//            await bus.Publish(eventData);
//            await scope.CompleteAsync();
//        }
//    }

//    public void Subscribe<TEvent, TEventHandler>() where TEventHandler : IEventHandler<TEvent>
//    {
//        var eventName = subsManager.GetEventKey<TEvent>();
//        DoInternalSubscription(eventName);

//        logger.LogInformation("Subscribing to event {EventName} with {EventHandler}",
//            eventName,
//            typeof(TEventHandler).GetGenericTypeName());

//        subsManager.AddSubscription<TEvent, TEventHandler>();

//        bus.Subscribe<TEvent>();
//    }

//    public void Unsubscribe<TEvent, TEventHandler>() where TEventHandler : IEventHandler<TEvent>
//    {
//        var eventName = subsManager.GetEventKey<TEvent>();
//        subsManager.RemoveSubscription<TEvent, TEventHandler>();
//        logger.LogInformation("Unsubscribing from event {EventName}", eventName);


//        bus.Unsubscribe<TEvent>();
//    }

//    public async Task ProcessEventAsync(string eventName,
//        object? eventData)
//    {
//        logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

//        if (subsManager.HasSubscriptionsForEvent(eventName))
//        {
//            var subscriptions = subsManager.GetHandlersForEvent(eventName);
//            foreach (var subscription in subscriptions)
//            {
//                {
//                    var handler = (IEventHandler)serviceProvider.GetRequiredService(subscription.HandlerType);

//                    var eventType = subsManager.GetEventTypeByName(eventName);
//                    if (eventType == null)
//                    {
//                        logger.LogWarning("Get event type failed,eventName={EventName}", eventName);
//                        continue;
//                    }
//                    if (eventData == null)
//                    {
//                        logger.LogWarning("Deserialize data to type `{Type}` failed,json data is `{Data}`",
//                            eventType,
//                            eventData);
//                        continue;
//                    }

//                    await eventHandlerInvoker.InvokeAsync(handler, eventData, eventType);
//                }
//            }
//        }
//        else
//        {
//            logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
//        }
//    }

//    private void DoInternalSubscription(string eventName)
//    {

//    }
//}