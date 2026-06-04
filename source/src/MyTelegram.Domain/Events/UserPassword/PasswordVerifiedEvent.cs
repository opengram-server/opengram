using MyTelegram.Domain.Aggregates.UserPassword;

namespace MyTelegram.Domain.Events.UserPassword;

/// <summary>
/// Event raised when password verification succeeds
/// </summary>
public class PasswordVerifiedEvent : RequestAggregateEvent2<UserPasswordAggregate, UserPasswordId>
{
    public PasswordVerifiedEvent(
        long userId,
        long srpId,
        RequestInfo requestInfo) : base(requestInfo)
    {
        UserId = userId;
        SrpId = srpId;
    }

    public long UserId { get; }
    public long SrpId { get; }
}
