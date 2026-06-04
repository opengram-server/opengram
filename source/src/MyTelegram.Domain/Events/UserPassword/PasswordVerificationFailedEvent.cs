using MyTelegram.Domain.Aggregates.UserPassword;

namespace MyTelegram.Domain.Events.UserPassword;

/// <summary>
/// Event raised when password verification fails
/// </summary>
public class PasswordVerificationFailedEvent : RequestAggregateEvent2<UserPasswordAggregate, UserPasswordId>
{
    public PasswordVerificationFailedEvent(
        long userId,
        long srpId,
        string reason,
        RequestInfo requestInfo) : base(requestInfo)
    {
        UserId = userId;
        SrpId = srpId;
        Reason = reason;
    }

    public long UserId { get; }
    public long SrpId { get; }
    public string Reason { get; }
}
