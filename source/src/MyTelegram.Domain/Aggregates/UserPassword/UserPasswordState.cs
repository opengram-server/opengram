using MyTelegram.Domain.Events.UserPassword;

namespace MyTelegram.Domain.Aggregates.UserPassword;

public class UserPasswordState : AggregateState<UserPasswordAggregate, UserPasswordId, UserPasswordState>,
    IApply<PasswordSetEvent>,
    IApply<PasswordRemovedEvent>,
    IApply<SrpSessionCreatedEvent>,
    IApply<PasswordVerifiedEvent>
{
    public long UserId { get; private set; }
    public bool HasPassword { get; private set; }
    public byte[]? Salt1 { get; private set; }
    public byte[]? Salt2 { get; private set; }
    public byte[]? V { get; private set; }
    public string? Hint { get; private set; }
    public string? Email { get; private set; }
    public int G { get; private set; }
    public byte[]? P { get; private set; }
    public long? SrpId { get; private set; }
    public byte[]? SrpB { get; private set; }
    public byte[]? SrpBPrivate { get; private set; }

    public void Apply(PasswordSetEvent aggregateEvent)
    {
        UserId = aggregateEvent.UserId;
        HasPassword = true;
        Salt1 = aggregateEvent.Salt1;
        Salt2 = aggregateEvent.Salt2;
        V = aggregateEvent.V;
        Hint = aggregateEvent.Hint;
        Email = aggregateEvent.Email;
        G = aggregateEvent.G;
        P = aggregateEvent.P;
    }

    public void Apply(PasswordRemovedEvent aggregateEvent)
    {
        UserId = aggregateEvent.UserId;
        HasPassword = false;
        Salt1 = null;
        Salt2 = null;
        V = null;
        Hint = null;
        Email = null;
        SrpId = null;
        SrpB = null;
        SrpBPrivate = null;
    }

    public void Apply(SrpSessionCreatedEvent aggregateEvent)
    {
        SrpId = aggregateEvent.SrpId;
        SrpB = aggregateEvent.SrpB;
        SrpBPrivate = aggregateEvent.SrpBPrivate;
    }

    public void Apply(PasswordVerifiedEvent aggregateEvent)
    {
        // After successful verification, clear the SRP session
        SrpId = null;
        SrpB = null;
        SrpBPrivate = null;
    }
}
