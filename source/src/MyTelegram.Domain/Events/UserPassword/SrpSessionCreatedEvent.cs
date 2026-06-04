using MyTelegram.Domain.Aggregates.UserPassword;

namespace MyTelegram.Domain.Events.UserPassword;

/// <summary>
/// Event raised when SRP session is created for password verification
/// </summary>
public class SrpSessionCreatedEvent : RequestAggregateEvent2<UserPasswordAggregate, UserPasswordId>
{
    public SrpSessionCreatedEvent(
        long userId,
        long srpId,
        byte[] srpB,
        byte[] srpBPrivate,
        RequestInfo requestInfo) : base(requestInfo)
    {
        UserId = userId;
        SrpId = srpId;
        SrpB = srpB;
        SrpBPrivate = srpBPrivate;
    }

    public long UserId { get; }
    public long SrpId { get; }
    public byte[] SrpB { get; }
    public byte[] SrpBPrivate { get; }
}
