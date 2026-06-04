namespace MyTelegram.Domain.Events.User;

public class CustomVerificationRemovedEvent : RequestAggregateEvent2<UserAggregate, UserId>
{
    public CustomVerificationRemovedEvent(
        RequestInfo requestInfo,
        long targetUserId) : base(requestInfo)
    {
        TargetUserId = targetUserId;
    }

    public long TargetUserId { get; }
}
