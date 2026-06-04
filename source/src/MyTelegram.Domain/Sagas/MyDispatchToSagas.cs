namespace MyTelegram.Domain.Sagas;

public class MyDispatchToSagas(
    ILogger<MyDispatchToSagas> logger,
    IServiceProvider serviceProvider,
    ISagaStore sagaStore,
    ISagaDefinitionService sagaDefinitionService,
    ISagaErrorHandler sagaErrorHandler,
    ISagaUpdateResilienceStrategy sagaUpdateLog,
    Func<Type, ISagaErrorHandler> sagaErrorHandlerFactory,
    IInMemorySagaStore inMemorySagaStore)
    : IDispatchToSagas
{
    public async Task ProcessAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken)
    {
        foreach (var domainEvent in domainEvents)
        {
            await ProcessAsync(
                    domainEvent,
                    cancellationToken)
                ;
        }
    }

    private async Task ProcessAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        var sagaTypeDetails = sagaDefinitionService.GetSagaDetails(domainEvent.EventType);

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace(
                "Saga types to process for domain event {DomainEventType}: {SagaTypes}",
                domainEvent.EventType.PrettyPrint(),
                sagaTypeDetails.Select(d => d.SagaType.PrettyPrint()));
        }

        foreach (var details in sagaTypeDetails)
        {
            var locator = (ISagaLocator)serviceProvider.GetRequiredService(details.SagaLocatorType);
            var sagaId = await locator.LocateSagaAsync(domainEvent, cancellationToken);

            if (sagaId == null)
            {
                logger.LogTrace(
                    "Saga locator {SagaLocatorType} returned null",
                    details.SagaLocatorType.PrettyPrint());
                continue;
            }

            await ProcessSagaAsync(domainEvent, sagaId, details, cancellationToken);
        }
    }

    private async Task ProcessSagaAsync(
        IDomainEvent domainEvent,
        ISagaId sagaId,
        SagaDetails details,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogTrace(
                "Loading saga {SagaType} with ID {Id}",
                details.SagaType.PrettyPrint(),
                sagaId);

            if (typeof(IInMemorySaga).IsAssignableFrom(details.SagaType))
            {
                await inMemorySagaStore.UpdateAsync(
                        sagaId,
                        details.SagaType,
                        domainEvent.Metadata.EventId,
                        (s,
                            c) => UpdateSagaAsync(s, domainEvent, details, c),
                        cancellationToken)
                    ;
            }
            else
            {
                await sagaStore.UpdateAsync(
                        sagaId,
                        details.SagaType,
                        domainEvent.Metadata.EventId,
                        (s,
                            c) => UpdateSagaAsync(s, domainEvent, details, c),
                        cancellationToken)
                    ;
            }
        }
        catch (Exception e)
        {
            // Search for a specific SagaErrorHandler<Saga> based on saga type
            var specificSagaErrorHandler = sagaErrorHandlerFactory(details.SagaType);

            var handled = specificSagaErrorHandler != null
                ? await specificSagaErrorHandler.HandleAsync(sagaId, details, e, cancellationToken)
                    .ConfigureAwait(false)
                : await sagaErrorHandler.HandleAsync(sagaId, details, e, cancellationToken);

            if (handled)
            {
                return;
            }

            logger.LogError(
                "Failed to process domain event {DomainEventType} for saga {SagaType}",
                domainEvent.EventType,
                details.SagaType.PrettyPrint());
            throw;
        }
    }

    private async Task UpdateSagaAsync(
        ISaga saga,
        IDomainEvent domainEvent,
        SagaDetails details,
        CancellationToken cancellationToken)
    {
        if (saga.State == SagaState.Completed)
        {
            logger.LogTrace(
                "Saga {SagaType} is completed, skipping processing of {DomainEventType}",
                details.SagaType.PrettyPrint(),
                domainEvent.EventType.PrettyPrint());
            return;
        }

        if (saga.State == SagaState.New && !details.IsStartedBy(domainEvent.EventType))
        {
            logger.LogTrace(
                "Saga {SagaType} isn't started yet and not started by {DomainEventType}, skipping",
                details.SagaType.PrettyPrint(),
                domainEvent.EventType.PrettyPrint());
            return;
        }

        var sagaUpdaterType = typeof(ISagaUpdater<,,,>).MakeGenericType(
            domainEvent.AggregateType,
            domainEvent.IdentityType,
            domainEvent.EventType,
            details.SagaType);
        var sagaUpdater = (ISagaUpdater)serviceProvider.GetRequiredService(sagaUpdaterType);

        await sagaUpdateLog.BeforeUpdateAsync(
                saga,
                domainEvent,
                details,
                cancellationToken)
            ;
        try
        {
            await sagaUpdater.ProcessAsync(
                    saga,
                    domainEvent,
                    SagaContext.Empty,
                    cancellationToken)
                ;
            await sagaUpdateLog.UpdateSucceededAsync(
                    saga,
                    domainEvent,
                    details,
                    cancellationToken)
                ;
        }
        catch (Exception e)
        {
            if (!await sagaUpdateLog.HandleUpdateFailedAsync(
                        saga,
                        domainEvent,
                        details,
                        e,
                        cancellationToken)
                    .ConfigureAwait(false))
            {
                throw;
            }
        }
    }
}