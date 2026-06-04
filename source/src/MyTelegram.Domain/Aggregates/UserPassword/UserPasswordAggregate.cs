using MyTelegram.Domain.Events.UserPassword;

namespace MyTelegram.Domain.Aggregates.UserPassword;

public class UserPasswordAggregate : AggregateRoot<UserPasswordAggregate, UserPasswordId>
{
    private readonly UserPasswordState _state = new();

    public UserPasswordAggregate(UserPasswordId id) : base(id)
    {
        Register(_state);
    }

    private UserPasswordState State => _state;

    public void SetPassword(
        long userId,
        byte[] salt1,
        byte[] salt2,
        byte[] v,
        string? hint,
        string? email,
        int g,
        byte[] p,
        RequestInfo requestInfo)
    {
        var @event = new PasswordSetEvent(
            userId,
            salt1,
            salt2,
            v,
            hint,
            email,
            g,
            p,
            requestInfo);

        Emit(@event);
    }

    public void RemovePassword(long userId, RequestInfo requestInfo)
    {
        if (!State.HasPassword)
        {
            return;
        }

        var @event = new PasswordRemovedEvent(userId, requestInfo);
        Emit(@event);
    }

    public void CreateSrpSession(
        long userId,
        long srpId,
        byte[] srpB,
        byte[] srpBPrivate,
        RequestInfo requestInfo)
    {
        var @event = new SrpSessionCreatedEvent(
            userId,
            srpId,
            srpB,
            srpBPrivate,
            requestInfo);

        Emit(@event);
    }

    public void VerifyPassword(
        long userId,
        long srpId,
        RequestInfo requestInfo)
    {
        if (!State.HasPassword)
        {
            var failEvent = new PasswordVerificationFailedEvent(
                userId,
                srpId,
                "No password set",
                requestInfo);
            Emit(failEvent);
            return;
        }

        if (State.SrpId != srpId)
        {
            var failEvent = new PasswordVerificationFailedEvent(
                userId,
                srpId,
                "Invalid SRP ID",
                requestInfo);
            Emit(failEvent);
            return;
        }

        var @event = new PasswordVerifiedEvent(userId, srpId, requestInfo);
        Emit(@event);
    }
}
