namespace MyTelegram.Domain.Events.User;

public class UserNameUpdatedEvent(
    RequestInfo requestInfo,
    UserItem userItem,
    string? oldUserName,
    int date
    )
    : RequestAggregateEvent2<UserAggregate, UserId>(requestInfo)
{
    public string? OldUserName { get; } = oldUserName;
    public int Date { get; } = date;
    public UserItem UserItem { get; } = userItem;
}
