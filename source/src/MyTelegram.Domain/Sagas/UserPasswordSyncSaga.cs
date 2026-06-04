using MyTelegram.Domain.Aggregates.UserPassword;
using MyTelegram.Domain.Events.UserPassword;
using MyTelegram.Domain.Sagas.Identities;

namespace MyTelegram.Domain.Sagas;

public class UserPasswordSyncSaga :
    AggregateSaga<UserPasswordSyncSaga, UserPasswordSyncSagaId, UserPasswordSyncSagaLocator>,
    ISagaIsStartedBy<UserPasswordAggregate, UserPasswordId, PasswordSetEvent>,
    ISagaIsStartedBy<UserPasswordAggregate, UserPasswordId, PasswordRemovedEvent>
{
    public UserPasswordSyncSaga(UserPasswordSyncSagaId id) : base(id)
    {
    }

    public Task HandleAsync(IDomainEvent<UserPasswordAggregate, UserPasswordId, PasswordSetEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        var command = new Commands.User.UpdateUserPasswordStatusCommand(
            UserId.Create(evt.UserId),
            evt.RequestInfo,
            true);
        
        Publish(command);
        Complete();
        return Task.CompletedTask;
    }

    public Task HandleAsync(IDomainEvent<UserPasswordAggregate, UserPasswordId, PasswordRemovedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        var command = new Commands.User.UpdateUserPasswordStatusCommand(
            UserId.Create(evt.UserId),
            evt.RequestInfo,
            false);
        
        Publish(command);
        Complete();
        return Task.CompletedTask;
    }
}
