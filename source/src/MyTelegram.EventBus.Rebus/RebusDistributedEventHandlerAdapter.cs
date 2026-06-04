//using Rebus.Handlers;

//namespace MyTelegram.EventBus.Rebus;

//public class RebusDistributedEventHandlerAdapter<TEventData>(
//    RebusEventBus rebusDistributedEventBus,
//    IEventBusSubscriptionsManager eventBusSubscriptionsManager)
//    : IHandleMessages<TEventData>, IRebusDistributedEventHandlerAdapter
//{
//    protected RebusEventBus RebusDistributedEventBus { get; } = rebusDistributedEventBus;

//    public async Task Handle(TEventData message)
//    {
//        var eventName = eventBusSubscriptionsManager.GetEventKey(typeof(TEventData));
//        await RebusDistributedEventBus.ProcessEventAsync(eventName, message);
//    }
//}