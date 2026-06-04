namespace MyTelegram.Domain.Sagas.Identities;

public class EditAdminSagaLocator : DefaultSagaLocator<EditAdminSaga, EditAdminSagaId>
{
    //public Task<ISagaId> LocateSagaAsync(IDomainEvent domainEvent,
    //    CancellationToken cancellationToken)
    //{
    //    if (domainEvent.GetAggregateEvent() is not IHasCorrelationId id)
    //    {
    //        throw new NotSupportedException(
    //            $"Domain event:{domainEvent.GetAggregateEvent().GetType().FullName} should impl IHasCorrelationId ");
    //    }

    //    return Task.FromResult<ISagaId>(new EditAdminSagaId($"editadminsagaId-{id.CorrelationId}"));
    //}

    protected override EditAdminSagaId CreateSagaId(string requestId)
    {
        return new EditAdminSagaId(requestId);
    }
}
