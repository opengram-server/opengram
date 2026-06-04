using IInvokeAfterMsgProcessor = MyTelegram.Services.Services.IInvokeAfterMsgProcessor;

namespace MyTelegram.Messenger.CommandServer.DomainEventHandlers;

public class InvokeAfterMsgEventHandler(
    IInvokeAfterMsgProcessor invokeAfterMsgProcessor,
    ILogger<InvokeAfterMsgEventHandler> logger)
    : ISubscribeSynchronousToAll
{
    public async Task HandleAsync(IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken)
    {
        foreach (var domainEvent in domainEvents)
        {
            var aggregateEvent = domainEvent.GetAggregateEvent();
    
            var reqMsgId = 0L;
            switch (aggregateEvent)
            {
                case IHasRequestInfo requestInfo:
                    if (requestInfo.RequestInfo.AddRequestIdToCache)
                    {
                        reqMsgId = requestInfo.RequestInfo.ReqMsgId;
                    }

                    var timespan = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - requestInfo.RequestInfo.Date;
                    if (timespan > 500)
                    {
                        logger.LogDebug("Process domain event '{DomainEvent}' is too slow, timespan: {Timespan}ms, reqMsgId: {ReqMsgId}",
                            domainEvent.GetAggregateEvent().GetType().Name,
                            timespan,
                            requestInfo.RequestInfo.ReqMsgId);
                    }

                    break;
            }

            if (reqMsgId == 0)
            {
                continue;
            }

            invokeAfterMsgProcessor.AddToRecentMessageIdList(reqMsgId);
            await invokeAfterMsgProcessor.HandleAsync(reqMsgId);
        }
    }
}
