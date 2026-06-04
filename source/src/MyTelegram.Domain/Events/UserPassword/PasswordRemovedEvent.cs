using MyTelegram.Domain.Aggregates.UserPassword;

namespace MyTelegram.Domain.Events.UserPassword;

/// <summary>
/// Event raised when user's password is removed
/// </summary>
public class PasswordRemovedEvent : RequestAggregateEvent2<UserPasswordAggregate, UserPasswordId>
{
    public PasswordRemovedEvent(
        long userId,
        RequestInfo requestInfo) : base(requestInfo)
    {
        UserId = userId;
    }

    public long UserId { get; }
}
