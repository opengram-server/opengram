using MyTelegram.Domain.Aggregates.UserPassword;

namespace MyTelegram.Domain.Events.UserPassword;

public class PasswordSetEvent : RequestAggregateEvent2<UserPasswordAggregate, UserPasswordId>
{
    public PasswordSetEvent(
        long userId,
        byte[] salt1,
        byte[] salt2,
        byte[] v,
        string? hint,
        string? email,
        int g,
        byte[] p,
        RequestInfo requestInfo) : base(requestInfo)
    {
        UserId = userId;
        Salt1 = salt1;
        Salt2 = salt2;
        V = v;
        Hint = hint;
        Email = email;
        G = g;
        P = p;
    }

    public long UserId { get; }
    public byte[] Salt1 { get; }
    public byte[] Salt2 { get; }
    public byte[] V { get; }
    public string? Hint { get; }
    public string? Email { get; }
    public int G { get; }
    public byte[] P { get; }
}
