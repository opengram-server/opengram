namespace MyTelegram.Domain.Events.UserName;

public class UserNameChangedEvent(RequestInfo requestInfo, Peer peer, string? userName, string? oldUserName,int date) : RequestAggregateEvent2<UserNameAggregate, UserNameId>(requestInfo)
{
    public Peer Peer { get; } = peer;
    public string? UserName { get; } = userName;
    public string? OldUserName { get; } = oldUserName;
    public int Date { get; } = date;
}